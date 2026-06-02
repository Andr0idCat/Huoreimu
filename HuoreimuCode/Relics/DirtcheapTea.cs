using AyaMod.Huoreimu.Potions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace AyaMod.Huoreimu.Relic;

public class DirtcheapTea : HuoreimuRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPotion<DirtcheapTeaPotion>()];

    public override Task BeforeCombatStartLate()
    {
        Flash();
        return PotionCmd.TryToProcure<DirtcheapTeaPotion>(Owner);
    }
}