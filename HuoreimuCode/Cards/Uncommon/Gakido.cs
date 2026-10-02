using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public sealed class Gakido() : HuoreimuCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private int _combatsSeen;

    [SavedProperty]
    public int CombatsSeen
    {
        get => _combatsSeen;
        set
        {
            AssertMutable();
            _combatsSeen = value;
            DynamicVars[HuoreimuConstants.COMBAT_SEEN_KEY].BaseValue = value;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new HealVar(10),
        new(HuoreimuConstants.MAX_COMBAT_KEY, 2),
        new(HuoreimuConstants.COMBAT_SEEN_KEY, CombatsSeen)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);

        if (IsInEliteOrBossRoom)
        {
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.IntValue);
        }
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType is not (RoomType.Elite or RoomType.Boss)) return;

        var pile = Pile;
        if (pile is not { Type: PileType.Deck }) return;

        CombatsSeen++;
        if (CombatsSeen >= DynamicVars[HuoreimuConstants.MAX_COMBAT_KEY].IntValue)
        {
            await CardPileCmd.RemoveFromDeck(this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HuoreimuConstants.MAX_COMBAT_KEY].UpgradeValueBy(2);
    }
}