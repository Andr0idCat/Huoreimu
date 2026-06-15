using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Musoutensei() : HuoreimuCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var rng = Owner.RunState.Rng.Niche;
        CardModel[] options = [ModelDb.Card<Normality>(), ModelDb.Card<Apotheosis>()];
        var template = rng.NextItem(options);
        var card = CombatState.CreateCard(template, Owner);
        if (IsUpgraded && card.IsUpgradable) CardCmd.Upgrade(card, CardPreviewStyle.None);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
    }
}
