using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class SfairaOsia() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int MIN_SELECTION = 0;
    private const int MAX_SELECTION = 2;

    private static readonly LocString ToHandPrompt = new LocString("cards", "AYAMOD-SFAIRA_OSIA.selectionScreenPrompt1");
    private static readonly LocString ToDrawPrompt = new LocString("cards", "AYAMOD-SFAIRA_OSIA.selectionScreenPrompt2");

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var discardCards = PileType.Discard.GetPile(Owner).Cards;
        if (discardCards.Count > 0)
        {
            var handPrefs = new CardSelectorPrefs(ToHandPrompt, MIN_SELECTION, MAX_SELECTION);
            var selectedToHand = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, handPrefs);
            if (selectedToHand.Any())
            {
                await CardPileCmd.Add(selectedToHand, PileType.Hand);
            }
        }

        discardCards = PileType.Discard.GetPile(Owner).Cards;
        if (discardCards.Count > 0)
        {
            var drawPrefs = new CardSelectorPrefs(ToDrawPrompt, MIN_SELECTION, MAX_SELECTION);
            var selectedToDraw = await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, drawPrefs);
            if (selectedToDraw.Any())
            {
                await CardPileCmd.Add(selectedToDraw, PileType.Draw, CardPilePosition.Top);
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
