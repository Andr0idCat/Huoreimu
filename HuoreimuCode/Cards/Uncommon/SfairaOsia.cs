using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class SfairaOsia() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private static readonly LocString ToHandPrompt = new LocString("cards", "AYAMOD-SFAIRA_OSIA.selectionScreenPrompt1");
    private static readonly LocString ToDrawPrompt = new LocString("cards", "AYAMOD-SFAIRA_OSIA.selectionScreenPrompt2");

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await CommonActions.CardBlock(this, cardPlay);

        var discardCards = PileType.Discard.GetPile(Owner).Cards.ToList();
        if (discardCards.Count == 0) return;

        var toHand = await CommonActions.SelectSingleCard(this, ToHandPrompt, choiceContext, PileType.Discard);
        if (toHand != null) await CardPileCmd.Add(toHand, PileType.Hand);

        var remaining = PileType.Discard.GetPile(Owner).Cards.Where(c => c != toHand).ToList();
        if (remaining.Count == 0) return;

        var toDraw = await CommonActions.SelectSingleCard(this, ToDrawPrompt, choiceContext, PileType.Discard);
        if (toDraw != null) await CardPileCmd.Add(toDraw, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
