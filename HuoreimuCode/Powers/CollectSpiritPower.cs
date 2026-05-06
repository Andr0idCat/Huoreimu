using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace AyaMod.Huoreimu.Powers;

public class CollectSpiritPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side is not CombatSide.Player) return;

        var enemies = combatState.Enemies;
        foreach (var enemy in enemies)
        {
            Flash();
            await PowerCmd.Apply<OmenOfDeathPower>(enemy, Amount, Owner, null);
        }
    }
}