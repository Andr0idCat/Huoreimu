using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AyaMod.Huoreimu.Cards.Common;

public sealed class OniBreaker() : HuoreimuCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    private int BaseVulnerable => IsUpgraded ? 2 : 1;
    private const int INCREASE_AMOUNT = 1;

    private int _eliteDefeated;

    [SavedProperty]
    public int EliteDefeated
    {
        get => _eliteDefeated;
        set
        {
            AssertMutable();
            _eliteDefeated = value;
            RefreshPowerAmount();
        }
    }

    private int TotalVulnerable => BaseVulnerable + EliteDefeated;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(TotalVulnerable),
        new PowerVar<WeakPower>(TotalVulnerable),
        new(HuoreimuConstants.INCREASE_VALUE, INCREASE_AMOUNT),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CommonActions.Apply<VulnerablePower>(choiceContext, cardPlay.Target, this);

        if (IsInEliteRoom)
        {
            await CommonActions.Apply<WeakPower>(choiceContext, cardPlay.Target, this);
        }
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType == RoomType.Elite && Pile?.Type == PileType.Deck) EliteDefeated += INCREASE_AMOUNT;
        return base.AfterCombatVictory(room);
    }

    protected override void OnUpgrade()
    {
        RefreshPowerAmount();
    }

    private void RefreshPowerAmount()
    {
        DynamicVars.Power<VulnerablePower>().BaseValue = TotalVulnerable;
        DynamicVars.Power<WeakPower>().BaseValue = TotalVulnerable;
    }
}