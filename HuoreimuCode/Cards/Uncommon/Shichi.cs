using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Tags;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards;

public class Shichi() : HuoreimuCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new RepeatVar(2),
        new UltimateSkillVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await UltimateSkillAction.EarlyExecute(this, cardPlay, () => OnCast(choiceContext),
            () => OnFail(choiceContext, cardPlay));
    }

    private Task OnCast(PlayerChoiceContext choiceContext)
    {
        if (CombatState == null)
        {
            Log.Warn($"{nameof(Shichi)}: CombatState is null, skipping attack.");
            return Task.CompletedTask;
        }

        var instance = CommonActions.CardAttack(this, null, DynamicVars.Damage.IntValue,
            DynamicVars.Repeat.IntValue);
        instance.TargetingAllOpponents(CombatState);
        return instance.Execute(choiceContext);
    }

    private Task OnFail(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return CommonActions.CardAttack(this, cardPlay.Target, DynamicVars.Damage.IntValue, DynamicVars.Repeat.IntValue)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}