using System.Collections.Generic;
using System.Threading.Tasks;
using AyaMod.Huoreimu.Character;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AyaMod.Huoreimu.Relic;

public class FoulBlood : HuoreimuRelic
{
    private const string DECREASE = "Decrease";
    private const string MINIMUM = "Minimum";
    private const int INITIAL_BONUS = 4;
    private const int DECREASE_AMOUNT = 1;
    private const int MINIMUM_AMOUNT = 0;

    private int _bonus = INITIAL_BONUS;

    [SavedProperty]
    public int Bonus
    {
        get => _bonus;
        set
        {
            AssertMutable();
            _bonus = value;
            DynamicVars.Power<StrengthPower>().BaseValue = value;
            DynamicVars.Power<DexterityPower>().BaseValue = value;
            Status = value <= 0 ? RelicStatus.Disabled : RelicStatus.Active;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => true;
    public override int DisplayAmount => Bonus;
    public override bool IsUsedUp => Bonus <= 0;

    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(Bonus),
        new PowerVar<DexterityPower>(Bonus),
        new(DECREASE, DECREASE_AMOUNT),
        new(MINIMUM, MINIMUM_AMOUNT),
    ];

    public override async Task BeforeCombatStart()
    {
        var room = Owner.RunState.CurrentRoom;
        if (room?.RoomType != RoomType.Elite) return;
        if (Bonus <= 0) return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, Bonus, Owner.Creature, null);
        await PowerCmd.Apply<DexterityPower>(Owner.Creature, Bonus, Owner.Creature, null);
    }

    public override Task AfterCombatVictoryEarly(CombatRoom room)
    {
        if (room.RoomType == RoomType.Elite && Bonus > MINIMUM_AMOUNT) Bonus -= DECREASE_AMOUNT;
        return Task.CompletedTask;
    }
}