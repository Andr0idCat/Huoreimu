using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public sealed class Ningendo() : HuoreimuCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int INITIAL_DAMAGE = 6;
    private int _damage = INITIAL_DAMAGE;

    [SavedProperty]
    public int SavedDamage
    {
        get => _damage;
        set
        {
            AssertMutable();
            _damage = value;
            DynamicVars.Damage.BaseValue = _damage;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(SavedDamage, ValueProp.Move),
        new IntVar(HuoreimuConstants.INCREASE_VALUE, 2),
        new IntVar(HuoreimuConstants.ELITE_OR_BOSS_INCREASE_VALUE, 8),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());

        var attack = await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);

        if (!shouldTriggerFatal) return;
        if (attack.Results.SelectMany(r => r).All(r => !r.WasTargetKilled)) return;

        var room = CombatState?.RunState.CurrentRoom;
        ArgumentNullException.ThrowIfNull(room);
        var roomType = room.RoomType;
        switch (roomType)
        {
            case RoomType.Monster:
            case RoomType.Elite:
            case RoomType.Boss:
                SavedDamage +=
                    DynamicVars.TryGetValue(
                        IsInEliteOrBossRoom
                            ? HuoreimuConstants.ELITE_OR_BOSS_INCREASE_VALUE
                            : HuoreimuConstants.INCREASE_VALUE, out var increaseVar)
                        ? increaseVar.IntValue
                        : 0;
                break;
            default:
                return;
        }

        CardCmd.Preview(this);
        if (DeckVersion is Ningendo deckVersion) deckVersion.SavedDamage = SavedDamage;
    }

    protected override void AfterDeserialized()
    {
        base.AfterDeserialized();
        DynamicVars.Damage.BaseValue = SavedDamage;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}