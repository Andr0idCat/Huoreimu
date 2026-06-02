using AyaMod.Huoreimu.Character;
using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;

namespace AyaMod.Huoreimu.Potions;

[Pool(typeof(HuoreimuPotionPool))]
public abstract class HuoreimuPotion : CustomPotionModel
{
    private readonly string _potionUid;

    protected HuoreimuPotion(PotionRarity rarity, PotionUsage usage, TargetType targetType)
    {
        Rarity = rarity;
        Usage = usage;
        TargetType = targetType;
        _potionUid = Id.Entry.RemovePrefix().ToLowerInvariant();
    }

    public override PotionRarity Rarity { get; }
    public override PotionUsage Usage { get; }
    public override TargetType TargetType { get; }

    public override string CustomPackedImagePath => $"{_potionUid}.png".PotionPath();
    public override string CustomPackedOutlinePath => $"{_potionUid}.png".PotionPath();
}