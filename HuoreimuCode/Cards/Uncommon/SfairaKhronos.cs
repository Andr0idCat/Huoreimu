using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class SfairaKhronos() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int MIN_SELECTION = 0;
    private const int MAX_SELECTION = 2;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new (HuoreimuConstants.MINIMUM_VALUE, MIN_SELECTION),
        new (HuoreimuConstants.MAXIMUM_VALUE, MAX_SELECTION)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await CommonActions.Draw(this, choiceContext);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, MIN_SELECTION, MAX_SELECTION);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => !c.ShouldRetainThisTurn, this);
        foreach (var card in selected)
        {
            card.GiveSingleTurnRetain();
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}