using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Rare;

public class GenocideKick() : HuoreimuCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(5),
        new ExtraDamageVar(1),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) => card.Owner.Creature.Block),
        new RepeatVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(choiceContext);
        await CreatureCmd.LoseBlock(choiceContext,Owner.Creature, Owner.Creature.Block, Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
    }
}