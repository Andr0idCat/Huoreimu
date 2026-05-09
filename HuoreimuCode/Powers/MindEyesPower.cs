using AyaMod.Huoreimu.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public class MindEyesPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OmenOfDeathPower>(),
    ];

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature applier,
        CardModel cardSource)
    {
        if (amount <= 0 || applier != Owner || power is not OmenOfDeathPower) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, amount * Amount, ValueProp.Unpowered, null);
    }
}