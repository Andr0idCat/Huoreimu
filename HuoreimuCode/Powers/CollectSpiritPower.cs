using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AyaMod.Huoreimu.Powers;

public class CollectSpiritPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side is not CombatSide.Player) return;

        var ctx = new ThrowingPlayerChoiceContext();
        foreach (var enemy in combatState.Enemies)
        {
            Flash();
            await PowerCmd.Apply<OmenOfDeathPower>(ctx, enemy, Amount, Owner, null, false);
        }
    }
}