using System.Text.Json;
using AyaMod.Huoreimu.ModInterop;
using Godot;
using HarmonyLib;
using Huoreimu.Huoreimu;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using FileAccess = Godot.FileAccess;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(ModelDb), "Init")]
public static class ModelDbInitPatch
{
    [HarmonyPostfix]
    private static void OnModelDbInit()
    {
        if (!ModManager.GetLoadedMods().Any(m => string.Equals(m.manifest?.id, "LogUploader"))) return;

        const string path = $"res://{MainFile.ModId}/Config/uploader_config.json";
        if (!ResourceLoader.Exists(path)) return;
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var json = file.GetAsText();
        var modVersion = ReadModVersion();
        LogUploaderInterop.RegisterUploaderByJson(nameof(AyaMod), MainFile.ModId, modVersion, json);
    }

    private static string ReadModVersion()
    {
        using var file = FileAccess.Open("res://Huoreimu.json", FileAccess.ModeFlags.Read);
        if (file == null) return "unknown";
        var json = file.GetAsText();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("version", out var v) ? v.GetString() ?? "unknown" : "unknown";
    }
}