using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Kyoushinkon() : HuoreimuCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var curseCount = PileType.Hand.GetPile(Owner).Cards.Count(c => c.Type == CardType.Curse);
        await PlayerCmd.GainEnergy(1 + curseCount, Owner);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
