using System.Collections.Generic;
using System.Linq;
using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Suieishin() : HuoreimuCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private static readonly LocString SelectPrompt = new LocString("cards", "AYAMOD-SUIEISHIN.selectionScreenPrompt");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var maxSelect = IsUpgraded ? 2 : 1;
        var prefs = new CardSelectorPrefs(SelectPrompt, maxSelect);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, _ => true, this)).ToList();
        if (selected.Count == 0) return;

        // Get or create the power instance for this card play
        var power = Owner.Creature.GetPower<SuieishinPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<SuieishinPower>(Owner.Creature, 1, Owner.Creature, this);
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
