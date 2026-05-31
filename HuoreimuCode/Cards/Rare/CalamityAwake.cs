using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Cards.Rare;

public class CalamityAwake() : HuoreimuExtraDeckCard<Huo>(5, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const int MAX_ENERGY_COST = 5;
    private const int DECREASE_COST = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Huo>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CalamityAwakePower>(1),
        new(HuoreimuConstants.DECREASE_VALUE, DECREASE_COST)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.ApplySelf<CalamityAwakePower>(choiceContext, this);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)
    {
        if (CombatState == null || Pile?.Type != PileType.Hand) return Task.CompletedTask;

        var state = Owner.PlayerCombatState;
        ArgumentNullException.ThrowIfNull(state);
        var curseCount = state.AllCards.Count(c => c.Type == CardType.Curse);
        EnergyCost.SetThisTurn(Math.Max(0, MAX_ENERGY_COST - curseCount * DECREASE_COST), reduceOnly: true);
        CardsAction.Refresh(this);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}