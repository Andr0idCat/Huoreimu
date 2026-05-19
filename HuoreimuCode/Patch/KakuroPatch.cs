using AyaMod.Huoreimu.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]
public static class KakuroCostPatch
{
    public static void Postfix(CardEnergyCost __instance, ref int __result)
    {
        if (__result >= 0) return;

        var card = AccessTools.Field(typeof(CardEnergyCost), "_card").GetValue(__instance) as CardModel;
        if (card?.CombatState is null || card?.Type != CardType.Curse) return;

        var power = card.Owner?.Creature?.GetPower<KakuroPower>();
        if (power == null) return;

        __result = 0;
    }
}
