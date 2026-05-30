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
    private static readonly LocString RetainPrompt = new LocString("cards", "AYAMOD-SFAIRA_KHRONOS.selectionScreenPrompt");

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await CardPileCmd.Draw(choiceContext, 2, Owner);

        var prefs = new CardSelectorPrefs(RetainPrompt, 0, 2);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, _ => true, this);
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
