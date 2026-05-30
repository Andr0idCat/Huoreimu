using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class AkaiYume() : HuoreimuExtraDeckCard<Cards.Huo>(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AkaiYumePower>(4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Cards.Huo>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.ApplySelf<AkaiYumePower>(choiceContext, this, DynamicVars.Power<AkaiYumePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<AkaiYumePower>().UpgradeValueBy(2);
    }
}
