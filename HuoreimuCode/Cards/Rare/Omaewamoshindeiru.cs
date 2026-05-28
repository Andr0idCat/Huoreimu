using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Omaewamoshindeiru() : HuoreimuCard(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OmaewamoshindeiruPower>(10),
        new CalculationBaseVar(0),
        new CalculationExtraVar(10),
        new CalculatedVar(HuoreimuConstants.CALCULATED_HITS).WithMultiplier((_, target) =>
            target?.GetPower<OmenOfDeathPower>()?.Amount ?? 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OmenOfDeathPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        var target = cardPlay.Target;
        ArgumentNullException.ThrowIfNull(target);
        var hitCount = (int)((CalculatedVar)DynamicVars[HuoreimuConstants.CALCULATED_HITS]).Calculate(cardPlay.Target);
        await CommonActions.Apply<OmaewamoshindeiruPower>(choiceContext, target, this, hitCount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OmaewamoshindeiruPower>().UpgradeValueBy(5);
        DynamicVars.CalculationExtra.UpgradeValueBy(5);
    }
}