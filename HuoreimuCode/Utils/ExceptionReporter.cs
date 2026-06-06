using System.Text;
using System.Text.Json;
using Huoreimu.Huoreimu;
using MegaCrit.Sts2.Core.Debug;

namespace AyaMod.Huoreimu.Utils;

public static class ExceptionReporter
{
    private static readonly HttpClient Client = new();
    private static string _serverUrl;
    private static string _modVersion = "unknown";
    private static string _gameVersion = "unknown";

    public static void Initialize(string serverUrl)
    {
        _serverUrl = serverUrl;
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

        var logPayload = new
        {
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
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(_serverUrl, content).ConfigureAwait(false);
        }
        catch
        {
            // silently fail
        }
    }
}