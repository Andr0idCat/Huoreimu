using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class ViolateShot() : HuoreimuCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int BASE_DAMAGE = 10;
    private int ExtraDamage => IsUpgraded ? 5 : 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ViolateShotPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var power = Owner.Creature.GetPower<ViolateShotPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<ViolateShotPower>(Owner.Creature, 1, Owner.Creature, this);
        }
        var curseCount = power?.Amount ?? 0;
        var totalDamage = BASE_DAMAGE + curseCount * ExtraDamage;

        await CommonActions.CardAttack(this, cardPlay.Target, totalDamage).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
    }
}
