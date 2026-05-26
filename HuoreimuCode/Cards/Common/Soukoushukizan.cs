using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class Soukoushukizan() : HuoreimuCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new CalculationBaseVar(1),
        new CalculationExtraVar(1),
        new CalculatedVar(HuoreimuConstants.CALCULATED_HITS).WithMultiplier((card, _) =>
            PileType.Hand.GetPile(card.Owner).Cards.Count(c => c.Type == CardType.Curse))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hitCount = (int)((CalculatedVar)DynamicVars[HuoreimuConstants.CALCULATED_HITS]).Calculate(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay.Target, DynamicVars.Damage.IntValue, hitCount)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
