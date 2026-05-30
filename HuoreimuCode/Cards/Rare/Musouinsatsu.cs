using System.Linq;
using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Musouinsatsu() : HuoreimuCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private static readonly LocString SelectAttackPrompt = new LocString("cards", "AYAMOD-MUSOUINSATSU.selectionScreenPrompt");

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MusouinsatsuPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var maxSelect = IsUpgraded ? 2 : 1;
        var prefs = new CardSelectorPrefs(SelectAttackPrompt, maxSelect);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => c.Type == CardType.Attack, this);
        foreach (var card in selected)
        {
            card.GiveSingleTurnRetain();
        }

        await CommonActions.ApplySelf<MusouinsatsuPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<MusouinsatsuPower>().UpgradeValueBy(1);
    }
}
