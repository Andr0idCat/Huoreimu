using MegaCrit.Sts2.Core.Entities.Cards;

namespace AyaMod.Huoreimu.Cards;

public class Huo() : HuoreimuCard(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    protected override bool IsPlayable => false;
}