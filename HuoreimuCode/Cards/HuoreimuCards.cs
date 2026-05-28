using AyaMod.Huoreimu.Audio;
using AyaMod.Huoreimu.Character;
using AyaMod.Huoreimu.Extensions;
using AyaMod.Huoreimu.Utils;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;


namespace AyaMod.Huoreimu.Cards;

[Pool(typeof(HuoreimuCardPool))]
public abstract class HuoreimuCard :
    CustomCardModel
{
    protected HuoreimuCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type, rarity,
        target)
    {
        CardUid = Id.Entry.RemovePrefix().ToLowerInvariant();
    }

    public string CardUid { get; }

    protected virtual string AudioName => $"{CardUid}_sfx";
    protected virtual string UltimateSkillAudioName => $"{CardUid}_ultimate";

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!ModelDb.Singleton<AudioConfig>().ShouldPlayAudio(this)) return Task.CompletedTask;
        PlayCardAudio(AudioName);
        return Task.CompletedTask;
    }

    private static void PlayCardAudio(string audioName)
    {
        var path = $"{audioName}.wav".AudioPath();
        Log.Info($">>>[HuoreimuMod]AudioPath={path}");
        if (ResourceLoader.Exists(path)) HuoreimuAudio.Play(path);
        else Log.Warn($">>>[HuoreimuMod]Audio {path} not exist.");
    }

    public void PlayUltimateSkillAudio()
    {
        if (!ModelDb.Singleton<AudioConfig>().ShouldPlayUltimateSkill(this)) return;
        PlayCardAudio(UltimateSkillAudioName);
    }

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
            Log.Info($">>>[HuoreimuMod]CardPath={path}");
            return ResourceLoader.Exists(path) ? path : "card.png".BigCardImagePath();
        }
    }

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            Log.Info($">>>[HuoreimuMod]CardPath={path}");
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    //Optional and I'm not sure it's functional yet.
    public override string BetaPortraitPath
    {
        get
        {
            var path = $"Beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            Log.Info($">>>[HuoreimuMod]CardPath={path}");
            return ResourceLoader.Exists(path) ? path : "Beta/card.png".CardImagePath();
        }
    }

    protected bool IsInEliteRoom => CombatState?.RunState?.CurrentRoom?.RoomType is RoomType.Elite;

    protected bool IsInEliteOrBossRoom =>
        CombatState?.RunState?.CurrentRoom?.RoomType is RoomType.Elite or RoomType.Boss;
}