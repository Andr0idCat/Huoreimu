using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Sekkatsukou() : HuoreimuCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private static IHoverTip[] NormalTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<FrailPower>(),
    ];

    private static IHoverTip[] UpgradedTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(3),
        new CardsVar(3),
        new PowerVar<StrengthPower>(3),
        new PowerVar<VulnerablePower>(99),
        new PowerVar<FrailPower>(99)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsUpgraded ? UpgradedTips : NormalTips;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await CommonActions.ApplySelf<StrengthPower>(choiceContext, this, DynamicVars.Power<StrengthPower>().BaseValue);
        await CommonActions.ApplySelf<VulnerablePower>(choiceContext, this,
            DynamicVars.Power<VulnerablePower>().BaseValue);
        if (IsUpgraded) return;
        await CommonActions.ApplySelf<FrailPower>(choiceContext, this, DynamicVars.Power<FrailPower>().BaseValue);
    }
}