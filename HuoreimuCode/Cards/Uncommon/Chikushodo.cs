using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public sealed class Chikushodo() : HuoreimuCard(INITIAL_COST, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int INITIAL_COST = 2;
    private int _eliteDefeatedAmount = 0;

    [SavedProperty]
    public int EliteDefeatedAmount
    {
        get => _eliteDefeatedAmount;
        set
        {
            AssertMutable();
            _eliteDefeatedAmount = value;
            EnergyCost.SetCustomBaseCost(CalculatedCost);
        }
    }
    
    private int CalculatedCost => Math.Max(0, INITIAL_COST - EliteDefeatedAmount);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new PowerVar<ChikushodoPower>(4),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CommonActions.Apply<ChikushodoPower>(choiceContext, cardPlay.Target, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }

    protected override void AfterDeserialized()
    {
        base.AfterDeserialized();
        EnergyCost.SetCustomBaseCost(CalculatedCost);
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType is not RoomType.Elite || CalculatedCost <= 0) return base.AfterCombatVictory(room);

        EliteDefeatedAmount += 1;
        CardCmd.Preview(this);
        return base.AfterCombatVictory(room);
    }
}