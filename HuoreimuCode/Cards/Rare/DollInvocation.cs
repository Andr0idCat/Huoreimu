using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class DollInvocation() : HuoreimuCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const int DOLL_COUNT = 2;
    private const int AUTO_PLAY_CARD_COST = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DollPower>(40),
        new(HuoreimuConstants.DOLL_COUNT, DOLL_COUNT)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DollPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (var i = 0; i < DOLL_COUNT; i++)
        {
            var power = await CommonActions.ApplySelf<DollPower>(choiceContext, this);
            if (power is null) return;
            power.AutoPlayCardCost = AUTO_PLAY_CARD_COST;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<DollPower>().UpgradeValueBy(40);
    }
}