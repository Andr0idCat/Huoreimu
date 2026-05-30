using System.Linq;
using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class DimensionalDisruption() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private static readonly LocString DiscardPrompt = new LocString("cards", "AYAMOD-DIMENSIONAL_DISRUPTION.selectionScreenPrompt");

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var hand = PileType.Hand.GetPile(Owner).Cards;
        var toDiscard = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(DiscardPrompt, 0, hand.Count), _ => true, this)).ToList();
        if (toDiscard.Count == 0) return;

        foreach (var card in toDiscard)
        {
            await CardCmd.Discard(choiceContext, card);
        }

        await CommonActions.ApplySelf<DimensionalDisruptionPower>(choiceContext, this, toDiscard.Count);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
