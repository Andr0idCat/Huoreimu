using AyaMod.Huoreimu.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]
public static class RelicObtainedPatch
{
    [HarmonyPostfix]
    private static async Task<RelicModel> Postfix(Task<RelicModel> __result, RelicModel relic, Player player)
    {
        var result = await __result;

        var listeners = PileType.Deck.GetPile(player).Cards.OfType<IRelicObtainedListener>();
        foreach (var listener in listeners)
        {
            await listener.OnRelicObtained(relic, player);
        }

        return result;
    }
}