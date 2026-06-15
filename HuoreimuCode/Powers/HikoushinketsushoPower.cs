using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public class HikoushinketsushoPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0, ValueProp.Unpowered)];

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier,
        CardModel cardSource)
    {
        if (power == this)
        {
            DynamicVars.Damage.BaseValue = Amount * 2;
        }

        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel drawnCard, bool fromHandDraw)
    {
        if (drawnCard.Owner != Owner.Player) return;
        if (drawnCard.Type is not (CardType.Curse or CardType.Status)) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies,
            drawnCard.Type is CardType.Status ? Amount : DynamicVars.Damage.IntValue, ValueProp.Unpowered, Owner,
            null);
    }
}