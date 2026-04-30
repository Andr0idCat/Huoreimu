using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace AyaMod.Huoreimu.Powers;

public class OmenOfDeathPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "omen_of_death.png".PowerIconPath();
    public override string CustomBigIconPath => "omen_of_death_big.png".PowerIconPath();
}