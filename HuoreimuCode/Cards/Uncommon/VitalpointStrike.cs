using System.Linq;
using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Tags;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class VitalpointStrike()
    : HuoreimuUltimateSkillCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, ValueProp.Move),
        new CardsVar(3),
        new SelectionVar(1),
        new UltimateSkillVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await UltimateSkillAction.Execute(choiceContext, this, cardPlay, () => OnCast(choiceContext, cardPlay),
            () => OnFallback(choiceContext, cardPlay));
    }

    private async Task OnCast(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        var cards = CardFactory.GetDistinctForCombat(Owner,
            Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Tags.Contains(HuoreimuTags.UltimateSkill)), DynamicVars.Cards.IntValue,
            Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in cards)
        {
            card.AddKeyword(HuoreimuKeywords.FreeUltimateSkill);
            card.AddKeyword(CardKeyword.Exhaust);
        }

        if (IsUpgraded) CardCmd.Upgrade(cards, CardPreviewStyle.None);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars[HuoreimuConstants.SELECTION_COUNT].IntValue);
        var selection = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, Owner, prefs)).ToArray();
        if (selection.Length <= 0) return;
        await CardPileCmd.AddGeneratedCardsToCombat(selection, PileType.Hand, Owner);
    }

    private async Task OnFallback(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }
}