using System.Reflection;
using AyaMod.Huoreimu.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch]
public static class KakuroCanPlayPatch
{
    private static MethodBase TargetMethod()
    {
        return typeof(CardModel).GetMethod("CanPlay", new[] { typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType() });
    }

    public static void Postfix(CardModel __instance, ref UnplayableReason reason, ref bool __result)
    {
        if (__result) return;
        if (__instance.Type != CardType.Curse) return;

        var power = __instance.Owner?.Creature?.GetPower<KakuroPower>();
        if (power == null) return;

        reason &= ~UnplayableReason.BlockedByCardLogic;
        __result = reason == UnplayableReason.None;
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]
public static class KakuroCostPatch
{
    public static void Postfix(CardEnergyCost __instance, ref int __result)
    {
        if (__result >= 0) return;

        var card = AccessTools.Field(typeof(CardEnergyCost), "_card").GetValue(__instance) as CardModel;
        if (card?.Type != CardType.Curse) return;

        var power = card.Owner?.Creature?.GetPower<KakuroPower>();
        if (power == null) return;

        __result = 0;
    }
}
