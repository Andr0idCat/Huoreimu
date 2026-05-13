using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public class OmaewamoshindeiruPower() : HuoreimuPowerBase(PowerType.Debuff, PowerStackType.Counter)
{
    private Player _player;

#nullable enable
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack()) return 1m;
        if (cardSource is null) return 1m;
        if (target != Owner) return 1m;
        return Amount / 100m + 1;
    }
#nullable disable

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return side is not CombatSide.Enemy
            ? base.AfterTurnEnd(choiceContext, side)
            : PowerCmd.Remove<OmaewamoshindeiruPower>(Owner);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (creature != Owner) return Task.CompletedTask;
        var currentRoom = CombatState.RunState.CurrentRoom;
        if (currentRoom is not CombatRoom combatRoom) return Task.CompletedTask;
        var player = _player;
        ArgumentNullException.ThrowIfNull(player);
        combatRoom.AddExtraReward(player,
            new CardReward(CardCreationOptions.ForRoom(player, combatRoom.RoomType), 3, player));
        return Task.CompletedTask;
    }
}