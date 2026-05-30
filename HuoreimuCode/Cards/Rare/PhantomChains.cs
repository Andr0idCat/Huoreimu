using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Rare;

public class PhantomChains() : HuoreimuCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int BASE_STRENGTH_LOSS = 2;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(BASE_STRENGTH_LOSS)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var amount = IsUpgraded ? 3 : 2;
        if (IsInEliteOrBossRoom) amount++;

        await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1);
    }
}
