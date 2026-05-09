using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public class MajinkaPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Type is not CardType.Curse) return;
        ArgumentNullException.ThrowIfNull(Owner.Player);
        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
        await PlayerCmd.GainEnergy(Amount, Owner.Player);
    }
}