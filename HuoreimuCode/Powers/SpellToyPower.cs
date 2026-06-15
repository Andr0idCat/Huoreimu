using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public class SpellToyPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side is not CombatSide.Player) return;
        var player = Owner.Player;
        if (player is null)
        {
            Log.Warn("SpellToyPower: Player is null, skipping block gain.");
            return;
        }

        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}