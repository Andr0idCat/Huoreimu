using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class LatentDisease() : HuoreimuCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int ADDITIONAL_VULNERABLE_POWER = 2;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new PowerVar<VulnerablePower>(1),
        new(HuoreimuConstants.ELITE_OR_BOSS_INCREASE_VALUE, ADDITIONAL_VULNERABLE_POWER)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        var amount = DynamicVars.Power<VulnerablePower>().IntValue + (IsInEliteOrBossRoom
            ? ADDITIONAL_VULNERABLE_POWER
            : 0);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}