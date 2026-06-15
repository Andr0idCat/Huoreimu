using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public sealed class CalamityAwakePower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Single)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack) return;
        if (cardPlay.Card.Owner != Owner.Player) return;

        Flash();
        var pool = Owner.Player.Character.CardPool.GetUnlockedCards(
            Owner.Player.UnlockState,
            Owner.Player.RunState.CardMultiplayerConstraint);

        var attacks = pool.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count == 0) return;

        var card = CardFactory.GetDistinctForCombat(Owner.Player, attacks, 1,
            Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (card == null) return;

        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player);
    }
}