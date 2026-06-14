using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Rare;

public class SinGraviton() : HuoreimuExtraDeckCard<Huo>(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(3),
        new PowerVar<VulnerablePower>(3),
        new PowerVar<OmenOfDeathPower>(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<OmenOfDeathPower>(),
        HoverTipFactory.FromCard<Huo>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<WeakPower>(choiceContext, enemy, this);
            await CommonActions.Apply<VulnerablePower>(choiceContext, enemy, this);
            await CommonActions.Apply<OmenOfDeathPower>(choiceContext, enemy, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<WeakPower>().UpgradeValueBy(2);
        DynamicVars.Power<VulnerablePower>().UpgradeValueBy(2);
        DynamicVars.Power<OmenOfDeathPower>().UpgradeValueBy(2);
    }
}