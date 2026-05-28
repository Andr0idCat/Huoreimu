using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Kasen() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Huo>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        var selection = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1),
            c => c is not Huo, this);
        var card = selection.FirstOrDefault();
        if (card is null) return;
        await CardCmd.TransformTo<Huo>(card);
        var deckCard = card.DeckVersion;
        if (deckCard is not null) await CardCmd.TransformTo<Huo>(deckCard);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        RemoveKeyword(CardKeyword.Ethereal);
    }
}