using System.Linq;
using AyaMod.Huoreimu.Relic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Odds;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(UnknownMapPointOdds), nameof(UnknownMapPointOdds.Roll))]
public static class HiddenDoomRollPatch
{
    public static void Postfix(IRunState runState, ref RoomType __result)
    {
        if (__result != RoomType.Monster) return;

        var hasRelic = runState.Players.Any(p => p.Relics.Any(r => r is HiddenDoom));
        if (!hasRelic) return;

        __result = RoomType.Elite;
    }
}
