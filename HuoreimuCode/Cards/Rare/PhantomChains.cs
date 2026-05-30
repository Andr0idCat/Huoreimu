using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Rare;

public class PhantomChains() : HuoreimuCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2),
        new(HuoreimuConstants.ELITE_OR_BOSS_INCREASE_VALUE, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var amount = DynamicVars.Power<StrengthPower>().IntValue;
        if (IsInEliteOrBossRoom)
        {
            var increase = DynamicVars.TryGetValue(HuoreimuConstants.ELITE_OR_BOSS_INCREASE_VALUE, out var dynamicVar)
                ? dynamicVar.IntValue
                : 0;
            amount += increase;
        }

        await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1);
    }
}