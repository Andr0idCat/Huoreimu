using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Basic;

public class SkywardKick() : HuoreimuCard(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6),
        new CalculationExtraVar(4),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((c, _) =>
        {
            if(c is not SkywardKick skywardKick) return 0;
            return skywardKick.IsInEliteOrBossRoom ? 1 : 0;
        }),
        new PowerVar<WeakPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        var target = cardPlay.Target;
        if (target is null) return;
        await CommonActions.Apply<WeakPower>(choiceContext, target, this);
        await CommonActions.CardBlock(this, DynamicVars.CalculatedBlock, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.Power<WeakPower>().UpgradeValueBy(1);
    }
}
