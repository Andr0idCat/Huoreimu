using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AyaMod.Huoreimu.Config;
using Huoreimu.Huoreimu;
using MegaCrit.Sts2.Core.Debug;

namespace AyaMod.Huoreimu.Utils;

public static class ExceptionReporter
{
    private static readonly HttpClient Client = new();
    private static string _serverUrl;
    private static byte[] _hmacSecretKey;
    private static string _modVersion = "unknown";
    private static string _gameVersion = "unknown";

    public static void Initialize(string serverUrl, string hmacSecretKey)
    {
        _serverUrl = serverUrl;
        _hmacSecretKey = Encoding.ASCII.GetBytes(hmacSecretKey);
        _modVersion = ReadModVersion();
        _gameVersion = ReadGameVersion();

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex) UploadAsync(ex).ConfigureAwait(false);
        };

        TaskScheduler.UnobservedTaskException += (_, args) => UploadAsync(args.Exception).ConfigureAwait(false);
    }

    private static string ReadModVersion()
    {
        using var file = Godot.FileAccess.Open("res://Huoreimu.json", Godot.FileAccess.ModeFlags.Read);
        if (file == null) return "unknown";
        var json = file.GetAsText();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("version", out var v) ? v.GetString() ?? "unknown" : "unknown";
    }

    private static string ReadGameVersion()
    {
        return ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? "unknown";
    }

    private static bool ContainsAyaMod(Exception ex)
    {
        var stack = ex.StackTrace;
        return stack != null && stack.Contains("AyaMod");
    }

    public static async Task UploadAsync(Exception ex)
    {
        if (string.IsNullOrEmpty(_serverUrl) || !ContainsAyaMod(ex)) return;
        if (!HuoreimuConfig.UploadErrorLogs) return;

        var logPayload = new
        {
            uuid = Guid.NewGuid().ToString(),
            mod_id = MainFile.ModId,
            mod_version = _modVersion,
            game_version = _gameVersion,
            error_message = ex.Message,
            stack_trace = ex.StackTrace ?? string.Empty,
            player_os = Environment.OSVersion.ToString(),
            created_at = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };

        try
        {
            var json = JsonSerializer.Serialize(logPayload);
            var messageBytes = Encoding.UTF8.GetBytes(json);
            using var hmac = new HMACSHA256(_hmacSecretKey);
            var hash = hmac.ComputeHash(messageBytes);
            var hashString = Convert.ToBase64String(hash);
            var request = new HttpRequestMessage(HttpMethod.Post, _serverUrl);
            request.Headers.Add("X-Mod-Signature", hashString);
            request.Content = new StringContent(json, Encoding.ASCII, "application/json");
            await Client.SendAsync(request).ConfigureAwait(false);
        }
        catch
        {
            // silently fail
        }
    }
}