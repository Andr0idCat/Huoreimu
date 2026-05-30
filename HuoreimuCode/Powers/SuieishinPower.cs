using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public sealed class SuieishinPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    private readonly List<string> _cardIds = new List<string>();

    public void AddCard(CardModel card)
    {
        if (!_cardIds.Contains(card.Id.Entry))
            _cardIds.Add(card.Id.Entry);
    }

    public override bool IsInstanced => true;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        if (_cardIds.Count == 0) return;

        Flash();
        var exhaust = PileType.Exhaust.GetPile(player);

        foreach (var id in _cardIds.ToList())
        {
            var card = exhaust.Cards.FirstOrDefault(c => c.Id.Entry == id);
            if (card == null) continue;

            _cardIds.Remove(id);
            if (card.Keywords.Contains(CardKeyword.Unplayable)) continue;

            await CardCmd.AutoPlay(choiceContext, card, null);
        }

        if (_cardIds.Count == 0) await PowerCmd.Remove(this);
    }
}
