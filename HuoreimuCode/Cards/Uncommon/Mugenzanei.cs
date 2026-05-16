using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Powers;
using AyaMod.Huoreimu.Tags;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Mugenzanei() : HuoreimuUltimateSkillCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new UltimateSkillVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MugenzaneiPower>(),
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (cardPlay.Target.GetPower<MugenzaneiPower>() != null)
        {
            await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
            return;
        }

        await UltimateSkillAction.EarlyExecute(this, cardPlay,
            async () =>
            {
                await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
                await CommonActions.Apply<MugenzaneiPower>(choiceContext, cardPlay.Target, this, 1);
            },
            () => CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
