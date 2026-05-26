using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Warp() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int MAX_EXHAUST_COUNT = 3;
    private const int MIN_CURSE_COUNT = 2;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>(1),
        new(HuoreimuConstants.MAX_EXHAUST_COUNT, MAX_EXHAUST_COUNT),
        new(HuoreimuConstants.MIN_CURSE_COUNT, MIN_CURSE_COUNT),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhausted = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, MAX_EXHAUST_COUNT), _ => true, this)).ToList();
        var selectedCount = exhausted.Count;
        foreach (var card in exhausted)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        await CardPileCmd.Draw(choiceContext, selectedCount, Owner);
        if (exhausted.Count(c => c.Type is CardType.Curse) >= MIN_CURSE_COUNT)
            await CommonActions.ApplySelf<IntangiblePower>(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}