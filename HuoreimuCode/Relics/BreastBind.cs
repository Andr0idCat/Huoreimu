using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace AyaMod.Huoreimu.Relic;

public class BreastBind : HuoreimuRelic
{
    private const int BUFFER_AMOUNT = 1;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>(BUFFER_AMOUNT)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BufferPower>()];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is not RoomType.Elite) return;
        var owner = Owner.Creature;
        await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), owner, BUFFER_AMOUNT, owner, null, false);
    }
}