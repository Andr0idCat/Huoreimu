using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Tags;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class BreakBones() : HuoreimuUltimateSkillCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new UltimateSkillVar(2),
        new PowerVar<WeakPower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await UltimateSkillAction.EarlyExecute(this, cardPlay,
            async () =>
            {
                await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
                await CommonActions.Apply<WeakPower>(choiceContext, cardPlay.Target, this);
            },
            () => CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Power<WeakPower>().UpgradeValueBy(1);
    }
}
