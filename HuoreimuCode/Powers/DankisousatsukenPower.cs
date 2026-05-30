using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public sealed class DankisousatsukenPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    private const int SELF_DAMAGE = 4;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;

        var hand = PileType.Hand.GetPile(Owner.Player);
        if (!hand.Cards.Any(c => c is Cards.Uncommon.Dankisousatsuken)) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, SELF_DAMAGE, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PowerCmd.ModifyAmount(this, 8, Owner, null);
    }
}
