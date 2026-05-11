using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Zekai() : HuoreimuCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>(1),
        new PowerVar<ArtifactPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<ArtifactPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntangiblePower>(choiceContext, this);
        await CommonActions.ApplySelf<ArtifactPower>(choiceContext, this);

        var statuses = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Type is CardType.Status)
            .ToList();
        foreach (var item in statuses)
        {
            await CardCmd.Exhaust(choiceContext, item);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<ArtifactPower>().UpgradeValueBy(2);
    }
}
