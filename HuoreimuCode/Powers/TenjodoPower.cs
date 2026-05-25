using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public class TenjodoPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props,
        Creature dealer, CardModel cardSource)
    {
        if (dealer is null || target != Owner) return Task.CompletedTask;
        Flash();
        return PowerCmd.Apply<OmenOfDeathPower>(dealer, Amount, Owner, null);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return side is not CombatSide.Enemy
            ? base.AfterTurnEnd(choiceContext, side)
            : PowerCmd.Remove<TenjodoPower>(Owner);
    }
}