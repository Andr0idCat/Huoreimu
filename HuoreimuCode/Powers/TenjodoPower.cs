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
        return PowerCmd.Apply<OmenOfDeathPower>(choiceContext, dealer, Amount, Owner, null, false);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        return side is not CombatSide.Enemy
            ? base.AfterSideTurnEnd(choiceContext, side, participants)
            : PowerCmd.Remove<TenjodoPower>(Owner);
    }
}