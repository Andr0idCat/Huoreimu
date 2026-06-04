using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Powers;
using AyaMod.Huoreimu.Tags;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Shoukaryumai() : HuoreimuUltimateSkillCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
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
        await UltimateSkillAction.EarlyExecute(this, cardPlay, async () =>
        {
            await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
            var cards = await CardsAction.AddCardToPileWhenCombat<Shoukaryumai>(Owner, PileType.Hand, 1, CombatState);
            if (IsUpgraded) CardCmd.Upgrade(cards, CardPreviewStyle.None);
        }, () => CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}