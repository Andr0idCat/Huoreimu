using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public class HikoushinketsushoPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel drawnCard, bool fromHandDraw)
    {
        if (drawnCard.Owner != Owner.Player) return;

        if (drawnCard.Type is CardType.Curse)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner, null);
            return;
        }

        if (drawnCard.Type is CardType.Status)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, Amount / 2, ValueProp.Unpowered, Owner, null);
        }
    }
}