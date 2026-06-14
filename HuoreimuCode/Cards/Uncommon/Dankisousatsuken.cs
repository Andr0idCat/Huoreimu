using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class Dankisousatsuken() : HuoreimuCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new ExtraDamageVar(3),
        new(HuoreimuConstants.INCREASE_VALUE, 8)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player || Pile?.Type != PileType.Hand) return;

        var increment = DynamicVars.TryGetValue(HuoreimuConstants.INCREASE_VALUE, out var increase)
            ? increase.IntValue
            : 0;
        DynamicVars.Damage.BaseValue += increment;
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.ExtraDamage.IntValue, ValueProp.Unpowered,
            null, this);
    }

    protected override void OnUpgrade()
    {
        if (!DynamicVars.TryGetValue(HuoreimuConstants.INCREASE_VALUE, out var increase)) return;
        increase.UpgradeValueBy(4);
    }
}