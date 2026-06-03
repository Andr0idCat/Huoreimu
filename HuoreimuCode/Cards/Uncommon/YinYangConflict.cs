using AyaMod.Huoreimu.Cards.Basic;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class YinYangConflict() : HuoreimuCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HuoreimuConstants.MAXIMUM_VALUE, 2),
        new CardsVar(2),
        new EnergyVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromCard<HuoreimuAttack>(),
        HoverTipFactory.FromCard<HuoreimuBlock>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        var maximum = DynamicVars.GetValueOrDefault(HuoreimuConstants.MAXIMUM_VALUE).IntValue;
        var prompt = SelectionScreenPrompt;
        prompt.Add(HuoreimuConstants.MAXIMUM_VALUE, maximum);
        var prefs = new CardSelectorPrefs(prompt, maximum);
        var selections = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, c => true, this))
            .GroupBy(c => c.GetType())
            .ToDictionary(g => g.Key, g => g.ToList());
        foreach (var card in selections.SelectMany(it => it.Value))
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        if (!selections.ContainsKey(typeof(HuoreimuAttack)) || !selections.ContainsKey(typeof(HuoreimuBlock))) return;
        await CommonActions.Draw(this, choiceContext);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}