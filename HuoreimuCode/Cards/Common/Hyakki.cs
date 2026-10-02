using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class Hyakki() : HuoreimuCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => CanCastAdditionalEffect;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9, ValueProp.Move),
        new PercentVar(25m),
        new IntVar(HuoreimuConstants.MAX_HP_LOSS, 1)
    ];

    private bool CanCastAdditionalEffect => (decimal)Owner.Creature.CurrentHp / Owner.Creature.MaxHp <=
                                            DynamicVars.GetPercent().PercentValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await CommonActions.CardBlock(this, cardPlay);
        if (!CanCastAdditionalEffect) return;

        await CreatureCmd.LoseMaxHp(choiceContext, Owner.Creature, DynamicVars[HuoreimuConstants.MAX_HP_LOSS].IntValue, true);
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}