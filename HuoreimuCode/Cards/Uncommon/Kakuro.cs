using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Kakuro() : HuoreimuCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private static readonly IHoverTip[] CustomHoverTips =
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    private static readonly IHoverTip[] UpgradedCustomHoverTips =
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<KakuroPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsUpgraded ? UpgradedCustomHoverTips : CustomHoverTips;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<KakuroPower>(choiceContext, this, IsUpgraded ? 2 : 1);
        power?.SetRetain(IsUpgraded);
    }
}