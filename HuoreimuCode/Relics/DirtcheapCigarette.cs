using AyaMod.Huoreimu.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AyaMod.Huoreimu.Relic;

public class DirtcheapCigarette : HuoreimuRelic
{
    private const int INTERVAL = 10;
    private const int STRENGTH_AMOUNT = 1;
    private int _omenOfDeathGiven;
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    [SavedProperty]
    public int OmenOfDeathGiven
    {
        get => _omenOfDeathGiven;
        set
        {
            AssertMutable();
            _omenOfDeathGiven = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => true;
    public override int DisplayAmount => OmenOfDeathGiven;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(STRENGTH_AMOUNT),
        new(HuoreimuConstants.INTERVAL, INTERVAL)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OmenOfDeathPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature applier, CardModel _)
    {
        var owner = Owner.Creature;
        if (applier != owner || power is not OmenOfDeathPower || amount < 0) return;
        var total = OmenOfDeathGiven + (int)amount;
        var (quotient, remainder) = int.DivRem(total, INTERVAL);
        OmenOfDeathGiven = remainder;
        if (quotient <= 0) return;
        await PowerCmd.Apply<StrengthPower>(owner, quotient * STRENGTH_AMOUNT, owner, null);
    }
}