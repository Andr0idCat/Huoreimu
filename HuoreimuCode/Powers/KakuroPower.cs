using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public sealed class KakuroPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Single)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BoolVar(nameof(CardKeyword.Retain))
    ];

    private BoolVar RetainVar => DynamicVars.TryGetValue(nameof(CardKeyword.Retain), out var dynamicVar)
        ? (BoolVar)dynamicVar
        : null;

    public void AddRetain(bool retain)
    {
        var retainVar = RetainVar;
        if (retainVar != null) retainVar.BoolVal = retain;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)
    {
        if (card is { Type: CardType.Curse, Pile: { Type: PileType.Hand } }) SetCard(card, RetainVar.BoolVal);
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature applier, CardModel cardSource)
    {
        var player = Owner.Player;
        ArgumentNullException.ThrowIfNull(player);
        var retain = RetainVar.BoolVal;
        foreach (var card in PileType.Hand.GetPile(player).Cards.Where(c => c.Type is CardType.Curse))
        {
            SetCard(card, retain);
        }

        return base.AfterApplied(applier, cardSource);
    }

    private static void SetCard(CardModel card, bool retain)
    {
        card.SetToFreeThisCombat();
        CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
        CardCmd.RemoveKeyword(card, retain ? [CardKeyword.Unplayable, CardKeyword.Retain] : [CardKeyword.Unplayable]);
    }
}