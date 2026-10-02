using System.Collections.Generic;
using System.Linq;
using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Suieishin() : HuoreimuCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const int MAX_SELECT = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(MAX_SELECT)];

    private static readonly LocString SelectPrompt = new LocString("cards", "AYAMOD-SUIEISHIN.selectionScreenPrompt");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var maxSelect = DynamicVars.Cards.IntValue;
        var prefs = new CardSelectorPrefs(SelectPrompt, maxSelect);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, _ => true, this)).ToList();
        if (selected.Count == 0) return;

        // Get or create the power instance for this card play
        var power = Owner.Creature.GetPower<SuieishinPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<SuieishinPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, false);
        }
        if (power == null) return;

        foreach (var card in selected)
        {
            power.AddCard(card);
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
