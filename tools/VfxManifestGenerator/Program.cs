using System.Text.Json;

var vfxDir = args.ElementAtOrDefault(0) ?? "";
var outputFile = args.ElementAtOrDefault(1) ?? "";

if (string.IsNullOrEmpty(vfxDir) || !Directory.Exists(vfxDir) || string.IsNullOrEmpty(outputFile))
{
    Console.WriteLine("VFX directory not found, skipping.");
    return;
}

var files = Directory.EnumerateFiles(vfxDir, "*.tscn", SearchOption.AllDirectories)
    .Select(f => Path.GetRelativePath(vfxDir, f).Replace('\\', '/'))
    .ToList();

var json = JsonSerializer.Serialize(files);
File.WriteAllText(outputFile, json);
Console.WriteLine($"Generated VFX manifest: {files.Count} files -> {outputFile}");
