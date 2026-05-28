using AyaMod.Huoreimu.Cards;
using BaseLib.Abstracts;
using Godot;
using Huoreimu.Huoreimu;
using FileAccess = Godot.FileAccess;

namespace AyaMod.Huoreimu.Utils;

public class AudioConfig : CustomSingletonModel
{
    #region inner

    private record struct Config
    {
        public bool PlayAudio { get; init; }
        public bool PlayUltimateSkill { get; init; }
    }

    #endregion

    private readonly Dictionary<string, Config> _configs = new();

    public AudioConfig() : base(false, false)
    {
        LoadConfigs();
    }

    public bool ShouldPlayAudio(HuoreimuCard card)
    {
        return _configs.TryGetValue(card.CardUid, out var config) && config.PlayAudio;
    }

    public bool ShouldPlayUltimateSkill(HuoreimuCard card)
    {
        return _configs.TryGetValue(card.CardUid, out var config) && config.PlayUltimateSkill;
    }

    private void LoadConfigs()
    {
        const string path = $"res://{MainFile.ModId}/Config/audio_config.json";
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var text = file.GetAsText();
        var dict = Json.ParseString(text).AsGodotDictionary();
        foreach (var (k, v) in dict)
        {
            var cardUid = k.AsString();
            var jsonConfig = v.AsGodotDictionary();
            _configs.Add(cardUid, new Config()
            {
                PlayAudio = jsonConfig.TryGetValue("sfx", out var sfx) && sfx.AsBool(),
                PlayUltimateSkill = jsonConfig.TryGetValue("ultimate", out var ultimate) && ultimate.AsBool(),
            });
        }
    }
}