using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public sealed class AkaiYumePower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(0, ValueProp.Unpowered)];

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature applier,
        CardModel cardSource)
    {
        if (power == this)
        {
            DynamicVars.Block.BaseValue = Amount * 2;
        }

        return base.AfterPowerAmountChanged(power, amount, applier, cardSource);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player) return;
        if (card.Type is not (CardType.Curse or CardType.Status)) return;

        Flash();
        await CreatureCmd.GainBlock(Owner, card.Type is CardType.Status ? Amount : DynamicVars.Block.IntValue,
            ValueProp.Move | ValueProp.Unpowered, null);
    }
}