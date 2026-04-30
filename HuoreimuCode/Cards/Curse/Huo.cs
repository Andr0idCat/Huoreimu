using MegaCrit.Sts2.Core.Entities.Cards;

namespace AyaMod.Huoreimu.Cards;

public class Huo() : HuoreimuCard(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
    protected override bool IsPlayable => false;
}