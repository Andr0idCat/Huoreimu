using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards;

public class SkywardKick() : HuoreimuCard(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
{
    private bool _isInEliteRoom;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6, ValueProp.Move),
        new PowerVar<WeakPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
        var target = cardPlay.Target;
        if (target is null || !_isInEliteRoom) return;
        await CommonActions.Apply<WeakPower>(choiceContext, target, this, DynamicVars.Weak.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        _isInEliteRoom = room.RoomType is RoomType.Elite;
        return base.AfterRoomEntered(room);
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _isInEliteRoom = false;
        return base.AfterCombatEnd(room);
    }
}