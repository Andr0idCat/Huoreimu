using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using AyaMod.Huoreimu.Actions;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Cards.Rare;

public class SunFall() : HuoreimuCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    // Default to unplayable; toggle this keyword when hand changes so the card
    // is playable only when the player has at least one Curse in hand.
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11, ValueProp.Move),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.CardAttack(this, null, DynamicVars.Damage.IntValue, 1)
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)
    {
        if (CombatState == null || Pile?.Type != PileType.Hand) return Task.CompletedTask;

        var hasCurse = PileType.Hand.GetPile(Owner).Cards.Any(c => c.Type == CardType.Curse);
        if (hasCurse)
            CardCmd.RemoveKeyword(this, [CardKeyword.Unplayable]);
        else
            CardCmd.ApplyKeyword(this, [CardKeyword.Unplayable]);

        CardsAction.Refresh(this);
        return Task.CompletedTask;
    }
}