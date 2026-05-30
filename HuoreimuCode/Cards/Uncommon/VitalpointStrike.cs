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
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class VitalpointStrike() : HuoreimuUltimateSkillCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, ValueProp.Move),
        new UltimateSkillVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);

        await UltimateSkillAction.Execute(this, cardPlay, async () =>
        {
            var pool = ModelDb.AllCards
                .Where(c => c.Tags.Contains(HuoreimuTags.UltimateSkill) && c != this)
                .ToList();
            if (pool.Count == 0) return;

            var rng = Owner.RunState.Rng.Niche;
            pool.StableShuffle(rng);
            var options = pool.Take(3).ToList();
            var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner, false);
            if (selected == null) return;

            var card = CombatState.CreateCard(selected, Owner);
            if (IsUpgraded && card.IsUpgradable) CardCmd.Upgrade(card, CardPreviewStyle.None);
            card.EnergyCost.SetThisCombat(0);
            CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        });
    }

    protected override void OnUpgrade()
    {
    }
}
