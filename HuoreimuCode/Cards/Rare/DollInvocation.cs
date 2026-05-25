using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class DollInvocation() : HuoreimuCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const int DOLL_COUNT = 2;
    private static readonly IHoverTip[] CommonTips = [HoverTipFactory.FromPower<DollPower>()];
    private static readonly IHoverTip[] UpgradedTips = [HoverTipFactory.FromPower<DollPowerUpgraded>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DollPower>(40),
        new PowerVar<DollPowerUpgraded>(40),
        new PowerVar<DollPowerVariant>(40),
        new PowerVar<DollPowerUpgradedVariant>(40),
        new(HuoreimuConstants.DOLL_COUNT, DOLL_COUNT)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsUpgraded ? UpgradedTips : CommonTips;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await CommonActions.ApplySelf<DollPowerUpgraded>(choiceContext, this);
            await CommonActions.ApplySelf<DollPowerUpgradedVariant>(choiceContext, this);
        }
        else
        {
            await CommonActions.ApplySelf<DollPower>(choiceContext, this);
            await CommonActions.ApplySelf<DollPowerVariant>(choiceContext, this);
        }
    }
}