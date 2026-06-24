using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public sealed class MusouinsatsuPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    private bool _attacked;

    private bool Attacked
    {
        get => _attacked;
        set
        {
            if (value == _attacked) return;
            _attacked = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => Attacked ? 0 : 1;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return Task.CompletedTask;
        Attacked = false;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || dealer == null || dealer == Owner || Attacked) return;

        var player = Owner.Player;
        if (player == null) return;

        var hand = PileType.Hand.GetPile(player);
        var attacks = hand.Cards.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count == 0) return;

        var card = player.RunState.Rng.CombatTargets.NextItem(attacks);
        Attacked = true;
        Flash();
        await CardCmd.AutoPlay(choiceContext, card, dealer);
    }
}