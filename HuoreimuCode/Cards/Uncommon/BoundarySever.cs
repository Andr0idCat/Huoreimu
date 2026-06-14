using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class BoundarySever() : HuoreimuCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int BASE_DAMAGE = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        if (cardPlay.Target.GetPower<IntangiblePower>() != null)
            await PowerCmd.Remove<IntangiblePower>(cardPlay.Target);

        var attack = await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        var dealt = attack.Results.Sum(r => r.UnblockedDamage + r.BlockedDamage);
        if (dealt > 0)
        {
            await PowerCmd.Apply<BoundarySeverPower>(cardPlay.Target, dealt, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
