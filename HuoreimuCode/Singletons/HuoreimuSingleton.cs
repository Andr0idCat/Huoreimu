using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Rooms;

namespace AyaMod.Huoreimu.Singletons;

public class HuoreimuSingleton() : CustomSingletonModel(true, true)
{
	public static bool IsInEliteRoom { get; private set; }

	public override Task AfterRoomEntered(AbstractRoom room)
	{
		IsInEliteRoom = room.RoomType is RoomType.Elite;
		return base.AfterRoomEntered(room);
	}

	public override Task AfterCombatEnd(CombatRoom room)
	{
		IsInEliteRoom = false;
		return base.AfterCombatEnd(room);
	}
}
