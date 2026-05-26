using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;

namespace AyaMod.Huoreimu.Character;

public class HuoreimuRelicPool : CustomRelicPoolModel
{
    public override string BigEnergyIconPath => "Charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "Charui/text_energy.png".ImagePath();
}
