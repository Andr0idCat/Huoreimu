using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace AyaMod.Huoreimu.Cards;

public class VoidGenesis() : HuoreimuExtraDeckCard<Huo>(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Huo>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var list = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Type is CardType.Curse or CardType.Status)
            .ToList();
        var exhaustCount = list.Count;
        foreach (var item in list)
        {
            await CardCmd.Exhaust(choiceContext, item);
        }

        var cards = CardFactory.GetForCombat(Owner,
            Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint), exhaustCount,
            Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var c in cards)
        {
            c.EnergyCost.SetThisCombat(c.EnergyCost.Canonical - 1);
        }

        if (IsUpgraded)
        {
            CardCmd.Upgrade(cards, CardPreviewStyle.None);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, addedByPlayer: true);
    }
}