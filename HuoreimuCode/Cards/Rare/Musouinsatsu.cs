using System.Linq;
using AyaMod.Huoreimu.DynamicVars;
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
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MusouinsatsuPower>(1),
        new SelectionVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var maxSelect = DynamicVars.TryGetValue(HuoreimuConstants.SELECTION_COUNT, out var selection)
            ? selection.IntValue
            : 0;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, maxSelect);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => c.Type == CardType.Attack, this);
        foreach (var card in selected)
        {
            card.AddKeyword(CardKeyword.Retain);
        }

        await CommonActions.ApplySelf<MusouinsatsuPower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        if(!DynamicVars.TryGetValue(HuoreimuConstants.SELECTION_COUNT, out var selection))return;
        selection.UpgradeValueBy(1);
    }
}