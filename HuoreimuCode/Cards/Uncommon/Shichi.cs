using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Tags;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards;

public class Shichi() : HuoreimuUltimateSkillCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
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
        await UltimateSkillAction.EarlyExecute(this, cardPlay, () => OnCast(choiceContext, cardPlay),
            () => OnFail(choiceContext, cardPlay));
    }

    private async Task OnCast(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        var target = cardPlay.Target;
        ArgumentNullException.ThrowIfNull(target);
        await using var context = await AttackCommand.CreateContextAsync(CombatState, this);
        var results =
            (await CommonActions.CardAttack(this, target, DynamicVars.Damage.IntValue, DynamicVars.Repeat.IntValue)
                .Execute(choiceContext)).Results.ToList();
        context.AddHit(results);
        var enemies = CombatState.GetTeammatesOf(target).Where(c => c.IsHittable && c != target).ToList();
        if (enemies.Count == 0) return;
        foreach (var result in results)
        {
            context.AddHit(await CreatureCmd.Damage(choiceContext, enemies, result.TotalDamage + result.OverkillDamage,
                ValueProp.Unpowered | ValueProp.Move, Owner.Creature, this));
        }
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