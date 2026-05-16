using System.Collections.Generic;
using System.Threading.Tasks;
using AyaMod.Huoreimu.Character;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace AyaMod.Huoreimu.Relic;

[Pool(typeof(HuoreimuRelicPool))]
public class ThirstyBloodJade : CustomRelicModel
{
    private const decimal HEAL_AMOUNT = 10;

    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(HEAL_AMOUNT)];

    public override async Task BeforeCombatStart()
    {
        var room = Owner.RunState.CurrentRoom;
        if (room?.RoomType == RoomType.Elite)
        {
            Flash();
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        }
    }

    public override async Task AfterCombatVictoryEarly(CombatRoom room)
    {
        if (room.RoomType == RoomType.Elite)
        {
            Flash();
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        }
    }
}
