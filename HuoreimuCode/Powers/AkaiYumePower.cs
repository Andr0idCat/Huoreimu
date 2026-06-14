using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public sealed class AkaiYumePower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player) return;

        if (card.Type is CardType.Curse)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);
            return;
        }

        if (card.Type is CardType.Status)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount / 2, ValueProp.Move, null);
        }
    }
}
