using System.Linq;
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
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return Task.CompletedTask;
        SetAmount(1);
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || dealer == null || Amount <= 0) return;

        var player = Owner.Player;
        if (player == null) return;

        var hand = PileType.Hand.GetPile(player);
        var attacks = hand.Cards.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count == 0) return;

        var card = player.RunState.Rng.CombatTargets.NextItem(attacks);
        Flash();
        SetAmount(0);
        await CardCmd.AutoPlay(choiceContext, card, dealer);
    }
}
