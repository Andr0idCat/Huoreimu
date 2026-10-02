using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class DimensionalDisruption() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 每多少张弃牌获得 1 点下回合能量
    private const int ENERGY_INTERVAL = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(HuoreimuConstants.INTERVAL, ENERGY_INTERVAL)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var hand = PileType.Hand.GetPile(Owner).Cards;
        var toDiscard = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, hand.Count), _ => true, this)).ToList();
        var discardAmount = toDiscard.Count;
        if (discardAmount == 0) return;

        foreach (var card in toDiscard)
        {
            await CardCmd.Discard(choiceContext, card);
        }

        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(choiceContext, this, discardAmount);
        // ReSharper disable once PossibleLossOfFraction
        await CommonActions.ApplySelf<EnergyNextTurnPower>(choiceContext, this, discardAmount / DynamicVars[HuoreimuConstants.INTERVAL].IntValue);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}