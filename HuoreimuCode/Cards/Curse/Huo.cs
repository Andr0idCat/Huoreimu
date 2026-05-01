using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace AyaMod.Huoreimu.Cards;

[Pool(typeof(CurseCardPool))]
public class Huo() : HuoreimuCard(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
    protected override bool IsPlayable => false;
}