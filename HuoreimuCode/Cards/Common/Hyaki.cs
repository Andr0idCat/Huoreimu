using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class Hyaki() : HuoreimuCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int BASE_BLOCK = 9;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var block = DynamicVars.Block.IntValue;
        await CommonActions.CardBlock(this, cardPlay);

        var hpPercent = (decimal)Owner.Creature.CurrentHp / Owner.Creature.MaxHp;
        if (hpPercent <= 0.25m)
        {
            await CreatureCmd.GainMaxHp(Owner.Creature, -1);
            await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
