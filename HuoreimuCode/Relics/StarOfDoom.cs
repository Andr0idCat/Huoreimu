using AyaMod.Huoreimu.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Relic;

public class StarOfDoom : HuoreimuRelic
{
    private const string COMBAT_START_POWER_COUNT = "CombatStartPowerCount";
    private const decimal ATTACK_OMEN = 1;
    private const decimal COMBAT_START_OMEN = 2;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OmenOfDeathPower>(ATTACK_OMEN),
        new(COMBAT_START_POWER_COUNT, COMBAT_START_OMEN)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OmenOfDeathPower>()
    ];

    public override async Task BeforeCombatStart()
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        Flash();
        await PowerCmd.Apply<OmenOfDeathPower>(new ThrowingPlayerChoiceContext(), combatState.HittableEnemies, COMBAT_START_OMEN, Owner.Creature, null, false);
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker != Owner.Creature) return;

        foreach (var result in command.Results.SelectMany(r => r))
        {
            // 只给敌方叠死兆星，避免自伤或友方伤害也生效
            if (result.Receiver.Side == Owner.Creature.Side) continue;

            Flash();
            await PowerCmd.Apply<OmenOfDeathPower>(choiceContext, result.Receiver, ATTACK_OMEN, Owner.Creature, null, false);
        }
    }

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature.Side == Owner.Creature.Side) return;

        var omen = creature.GetPower<OmenOfDeathPower>();
        var amount = omen?.Amount ?? 0;
        if (amount <= 0) return;

        var combatState = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        var targets = combatState.HittableEnemies.Where(e => e != creature).ToList();
        if (targets.Count == 0) return;

        var index = Owner.RunState.Rng.Niche.NextInt(0, targets.Count);
        Flash();
        await PowerCmd.Apply<OmenOfDeathPower>(new ThrowingPlayerChoiceContext(), targets[index], amount, Owner.Creature, null, false);
    }
}
