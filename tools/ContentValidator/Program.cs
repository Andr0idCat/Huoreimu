using System.Text.Json;
using System.Text.RegularExpressions;

// 内容校验器：在打包前检查本地化键、卡牌/能力等资源与音频配置是否自洽，
// 用来拦住"文件名拼错、漏翻译、键名对不上"这一类不会被编译器发现的问题。
//
// 用法：dotnet run --project tools/ContentValidator -- <项目根目录>

var root = Path.GetFullPath(args.ElementAtOrDefault(0) ?? Directory.GetCurrentDirectory());
var modId = "Huoreimu";
var languages = new[] { "zhs", "eng", "jpn", "kor" };

// 每种语言都应存在的本地化文件（文件名本身就是加载依据，拼错即失效）
var localizationFiles = new[]
{
    "ancients", "card_keywords", "cards", "characters", "enchantments",
    "potions", "powers", "relics", "settings_ui", "static_hover_tips"
};

// 内容类所在目录 -> 本地化文件
var contentFolders = new (string Folder, string LocalizationFile)[]
{
    (Path.Combine("HuoreimuCode", "Cards"), "cards"),
    (Path.Combine("HuoreimuCode", "Powers"), "powers"),
    (Path.Combine("HuoreimuCode", "Relics"), "relics"),
    (Path.Combine("HuoreimuCode", "Potions"), "potions"),
    (Path.Combine("HuoreimuCode", "Enchantment"), "enchantments")
};

// 不参与"内容类 <-> 本地化键"检查的本地化文件
var skipKeyChecks = new HashSet<string> { "ancients", "characters", "settings_ui", "static_hover_tips" };

var errors = new List<string>();
var warnings = new List<string>();
var knownIssues = LoadKnownIssues();

var localization = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
var localizationDir = Path.Combine(root, modId, "localization");

// ---------- 1. 本地化文件：解析、缺失、多余 ----------
foreach (var language in languages)
{
    var languageDir = Path.Combine(localizationDir, language);
    if (!Directory.Exists(languageDir))
    {
        Error($"缺少语言目录：{Relative(languageDir)}");
        continue;
    }

    localization[language] = new Dictionary<string, Dictionary<string, string>>();

    foreach (var name in localizationFiles)
    {
        var path = Path.Combine(languageDir, name + ".json");
        if (!File.Exists(path))
        {
            Error($"缺少本地化文件：{Relative(path)}");
            continue;
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
            localization[language][name] = parsed ?? new Dictionary<string, string>();
        }
        catch (Exception exception)
        {
            Error($"本地化文件不是合法的 JSON：{Relative(path)}（{exception.Message}）");
        }
    }

    foreach (var file in Directory.EnumerateFiles(languageDir, "*.json"))
    {
        var name = Path.GetFileNameWithoutExtension(file);
        if (!localizationFiles.Contains(name))
            Warn($"多余/命名可能写错的本地化文件：{Relative(file)}（不会被加载）");
    }
}

// ---------- 2. 各语言键集合必须一致 ----------
foreach (var name in localizationFiles)
{
    if (!localization.TryGetValue("zhs", out var reference) || !reference.TryGetValue(name, out var referenceKeys))
        continue;

    foreach (var language in languages)
    {
        if (language == "zhs") continue;
        if (!localization.TryGetValue(language, out var table) || !table.TryGetValue(name, out var keys))
            continue;

        var missing = referenceKeys.Keys.Except(keys.Keys).OrderBy(k => k).ToList();
        var extra = keys.Keys.Except(referenceKeys.Keys).OrderBy(k => k).ToList();
        if (missing.Count == 0 && extra.Count == 0) continue;

        Error($"{name}.json 的键与 zhs 不一致（{language}）：缺少 {missing.Count} 个、多出 {extra.Count} 个"
              + Describe(missing, extra));
    }
}

// ---------- 3. 内容类 <-> 本地化键 ----------
var customEnumValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach (var file in EnumerateSources(root))
{
    foreach (Match match in Regex.Matches(File.ReadAllText(file), @"\[CustomEnum\(""([^""]+)""\)\]"))
        customEnumValues.Add(Normalize(match.Groups[1].Value));
}

foreach (var (folder, localizationFile) in contentFolders)
{
    var directory = Path.Combine(root, folder);
    if (!Directory.Exists(directory))
    {
        Error($"缺少内容目录：{Relative(directory)}");
        continue;
    }

    var classKeys = new Dictionary<string, string>(); // 类名 -> 期望键
    foreach (var file in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
    {
        var text = File.ReadAllText(file);
        foreach (Match match in Regex.Matches(text,
                     @"^\s*(?:public|internal)\s+((?:sealed|abstract|partial|static)\s+)*class\s+([A-Za-z_][A-Za-z0-9_]*)",
                     RegexOptions.Multiline))
        {
            // 抽象基类没有自己的本地化文本
            if (match.Groups[1].Value.Contains("abstract")) continue;
            var className = match.Groups[2].Value;
            classKeys[className] = "AYAMOD-" + ToUpperSnake(className);
        }
    }

    if (!localization.TryGetValue("zhs", out var zhs) || !zhs.TryGetValue(localizationFile, out var keys2))
        continue;

    var keyGroups = keys2.Keys
        .Select(key => key.Contains('.') ? key[..key.LastIndexOf('.')] : key)
        .ToHashSet();

    // 3a. 每个内容类都要有 title / description
    foreach (var (className, expectedKey) in classKeys.OrderBy(pair => pair.Key))
    {
        if (!keyGroups.Contains(expectedKey))
        {
            Error($"{className} 缺少本地化键 {expectedKey}.*（{localizationFile}.json）");
            continue;
        }

        foreach (var suffix in new[] { "title", "description" })
        {
            if (!keys2.ContainsKey($"{expectedKey}.{suffix}"))
                Error($"{className} 缺少本地化键 {expectedKey}.{suffix}");
        }
    }

    // 3b. 不能存在没有对应类名的键（历史遗留键）
    if (skipKeyChecks.Contains(localizationFile)) continue;
    var knownClassNames = classKeys.Keys.Select(Normalize).ToHashSet();
    foreach (var group in keyGroups.OrderBy(g => g))
    {
        if (!group.StartsWith("AYAMOD-")) continue;
        var normalized = Normalize(group["AYAMOD-".Length..]);
        if (knownClassNames.Contains(normalized)) continue;
        Error($"{localizationFile}.json 中的键 {group}.* 找不到对应的类");
    }
}

// ---------- 4. 关键字键对应 [CustomEnum(...)] ----------
if (localization.TryGetValue("zhs", out var zhsTable) && zhsTable.TryGetValue("card_keywords", out var keywordKeys))
{
    var keywordGroups = keywordKeys.Keys
        .Select(key => key.Contains('.') ? key[..key.LastIndexOf('.')] : key)
        .Where(key => key.StartsWith("AYAMOD-"))
        .Select(key => key["AYAMOD-".Length..])
        .Distinct();
    foreach (var group in keywordGroups)
    {
        if (!customEnumValues.Contains(Normalize(group)))
            Error($"card_keywords.json 中的键 AYAMOD-{group}.* 找不到对应的 [CustomEnum] 定义");
    }
}

// ---------- 5. 音频配置 <-> 音频文件 ----------
var audioDir = Path.Combine(root, modId, "Audio");
var audioConfigPath = Path.Combine(root, modId, "Config", "audio_config.json");
var audioNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
if (File.Exists(audioConfigPath) && Directory.Exists(audioDir))
{
    Dictionary<string, AudioEntry> audioConfig = null;
    try
    {
        audioConfig = JsonSerializer.Deserialize<Dictionary<string, AudioEntry>>(
            File.ReadAllText(audioConfigPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch (Exception exception)
    {
        Error($"audio_config.json 不是合法的 JSON：{exception.Message}");
    }

    var audioFiles = Directory.EnumerateFiles(audioDir, "*.wav", SearchOption.AllDirectories)
        .Select(Path.GetFileName).ToList();

    if (audioConfig != null)
    {
        foreach (var (uid, entry) in audioConfig.OrderBy(pair => pair.Key))
        {
            if (entry.Sfx) CheckAudio($"{uid}_sfx.wav", uid, "sfx");
            if (entry.Ultimate) CheckAudio($"{uid}_ultimate.wav", uid, "ultimate");
        }
    }

    // 反向检查：磁盘上的音频必须被配置引用，文件名拼错时能立刻发现
    foreach (var file in audioFiles.OrderBy(f => f))
    {
        var baseName = Path.GetFileNameWithoutExtension(file);
        var suffixIndex = baseName.LastIndexOf('_');
        var uid = suffixIndex > 0 ? baseName[..suffixIndex] : baseName;
        var referenced = audioConfig != null && audioConfig.TryGetValue(uid, out var entry)
            && (baseName.EndsWith("_sfx") ? entry.Sfx : baseName.EndsWith("_ultimate") && entry.Ultimate);
        if (!referenced)
            Error($"音频文件没有被 audio_config.json 引用（文件名可能拼错）：{Relative(Path.Combine(audioDir, file))}");
    }
}

// ---------- 6. 本地化文本中的裸数字 ----------
// 规则：文本里来自代码的数值必须写成动态变量引用；只有 0/1（单位与结构性数字）默认允许，
// 其余数字需要在 allowed_numbers.txt 中显式登记，否则视为魔数。
var allowedNumbers = LoadAllowedNumbers();
foreach (var language in languages)
{
    if (!localization.TryGetValue(language, out var table)) continue;
    foreach (var (name, entries) in table)
    {
        foreach (var (key, value) in entries)
        {
            var stripped = Regex.Replace(value, @"\{[^{}]*\}", string.Empty);
            foreach (Match match in Regex.Matches(stripped, @"\d+"))
            {
                var digits = match.Value;
                if (digits is "0" or "1") continue;
                if (allowedNumbers.Contains($"{key}|{digits}")) continue;
                // 仅提示，不让构建失败：文本里的数值最好改为动态变量引用
                Warn($"{name}.json 中 {key} 含裸数字 {digits}，建议改为动态变量引用"
                     + "（确需保留请登记到 allowed_numbers.txt）");
            }
        }
    }
}

// ---------- 7. 输出 ----------
foreach (var warning in warnings) Console.WriteLine("[WARN ] " + warning);
foreach (var error in errors) Console.WriteLine("[ERROR] " + error);

Console.WriteLine();
Console.WriteLine($"校验完成：{errors.Count} 个错误，{warnings.Count} 个警告（语言 {languages.Length} 种，本地化文件 {localizationFiles.Length} 个）");
return errors.Count > 0 ? 1 : 0;

void CheckAudio(string fileName, string uid, string kind)
{
    var path = Path.Combine(audioDir, fileName);
    if (File.Exists(path)) return;
    Error($"audio_config.json 为 {uid} 开启了 {kind}，但缺少音频文件 {Relative(path)}");
}

void Error(string message)
{
    if (IsKnown(message))
    {
        warnings.Add(message + "（已知问题，见 known_issues.txt）");
        return;
    }

    errors.Add(message);
}

void Warn(string message)
{
    warnings.Add(message);
}

bool IsKnown(string message) => knownIssues.Any(issue => message.Contains(issue, StringComparison.OrdinalIgnoreCase));

List<string> LoadAllowedNumbers()
{
    var path = Path.Combine(root, "tools", "ContentValidator", "allowed_numbers.txt");
    if (!File.Exists(path)) return [];
    return File.ReadAllLines(path)
        .Select(line => line.Trim())
        .Where(line => line.Length > 0 && !line.StartsWith('#'))
        .ToList();
}

List<string> LoadKnownIssues()
{
    var path = Path.Combine(root, "tools", "ContentValidator", "known_issues.txt");
    if (!File.Exists(path)) return [];
    return File.ReadAllLines(path)
        .Select(line => line.Trim())
        .Where(line => line.Length > 0 && !line.StartsWith('#'))
        .ToList();
}

IEnumerable<string> EnumerateSources(string projectRoot)
{
    var codeDir = Path.Combine(projectRoot, "HuoreimuCode");
    return Directory.Exists(codeDir)
        ? Directory.EnumerateFiles(codeDir, "*.cs", SearchOption.AllDirectories)
        : [];
}

string Relative(string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

static string ToUpperSnake(string name) =>
    Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToUpperInvariant();

static string Normalize(string name) => name.Replace("_", string.Empty).ToLowerInvariant();

static string Describe(List<string> missing, List<string> extra)
{
    var parts = new List<string>();
    if (missing.Count > 0) parts.Add("缺少 " + string.Join(", ", missing.Take(8)) + (missing.Count > 8 ? " …" : string.Empty));
    if (extra.Count > 0) parts.Add("多出 " + string.Join(", ", extra.Take(8)) + (extra.Count > 8 ? " …" : string.Empty));
    return parts.Count == 0 ? string.Empty : "；" + string.Join("；", parts);
}

internal sealed class AudioEntry
{
    public bool Sfx { get; set; }
    public bool Ultimate { get; set; }
}
