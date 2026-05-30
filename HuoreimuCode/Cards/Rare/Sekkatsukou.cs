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
    private const int ENERGY_GAIN = 3;
    private const int DRAW_COUNT = 3;
    private const int STRENGTH = 3;
    private const int VULNERABLE = 99;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(ENERGY_GAIN),
        new CardsVar(DRAW_COUNT),
        new PowerVar<StrengthPower>(STRENGTH)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await PlayerCmd.GainEnergy(ENERGY_GAIN, Owner);
        await CardPileCmd.Draw(choiceContext, DRAW_COUNT, Owner);
        await CommonActions.ApplySelf<StrengthPower>(choiceContext, this, STRENGTH);
        await CommonActions.ApplySelf<VulnerablePower>(choiceContext, this, VULNERABLE);
    }

    protected override void OnUpgrade()
    {
    }
}
