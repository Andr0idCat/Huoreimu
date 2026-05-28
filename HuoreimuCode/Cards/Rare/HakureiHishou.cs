using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Rare;

public class HakureiHishou() : HuoreimuCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
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

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24, ValueProp.Move),
        new(HuoreimuConstants.MAX_COMBAT_KEY, 4),
        new(HuoreimuConstants.COMBAT_SEEN_KEY, CombatsSeen)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType is not (RoomType.Monster or RoomType.Elite or RoomType.Boss)) return;

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
        DynamicVars.Damage.UpgradeValueBy(12);
    }
}