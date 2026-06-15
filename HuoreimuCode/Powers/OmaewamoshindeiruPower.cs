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
#nullable enable
    private Player? _killer;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack()) return 1m;
        if (cardSource is null) return 1m;
        if (target != Owner) return 1m;
        return Amount / 100m + 1;
    }
#nullable disable

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        return side is not CombatSide.Enemy
            ? base.AfterSideTurnEnd(choiceContext, side, participants)
            : PowerCmd.Remove<OmaewamoshindeiruPower>(Owner);
    }

    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature dealer, DamageResult result, ValueProp props,
        Creature target, CardModel cardSource)
    {
        if (target != Owner) return Task.CompletedTask;
        _killer = result.WasTargetKilled ? dealer?.Player : null;
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (creature != Owner) return Task.CompletedTask;
        var currentRoom = CombatState.RunState.CurrentRoom;
        if (currentRoom is not CombatRoom combatRoom || _killer is null) return Task.CompletedTask;
        combatRoom.AddExtraReward(_killer,
            new CardReward(CardCreationOptions.ForRoom(_killer, combatRoom.RoomType), 3, _killer));
        return Task.CompletedTask;
    }
}