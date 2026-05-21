using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public sealed class DollPowerUpgraded : DollPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HuoreimuConstants.AUTO_PLAY_CARD_COST_MIN, 1),
        new(HuoreimuConstants.AUTO_PLAY_CARD_COST_MAX, 2),
    ];

    protected override bool CanAutoPlay(CardModel card)
    {
        return card.EnergyCost.Canonical is >= 1 and <= 2;
    }
}