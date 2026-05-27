using System.Reflection;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Tags;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SetToFreeThisCombat))]
public static class FreeUltimateApplyCombatPatch
{
    public static void Postfix(CardModel __instance)
    {
        if (__instance.Tags.Contains(HuoreimuTags.UltimateSkill))
            __instance.AddKeyword(HuoreimuKeywords.FreeUltimateSkill);
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SetToFreeThisTurn))]
public static class FreeUltimateApplyTurnPatch
{
    public static void Postfix(CardModel __instance)
    {
        if (__instance.Tags.Contains(HuoreimuTags.UltimateSkill))
            __instance.AddKeyword(HuoreimuKeywords.FreeUltimateSkill);
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.EndOfTurnCleanup))]
public static class FreeUltimateTurnCleanupPatch
{
    private static readonly FieldInfo CardField = typeof(CardEnergyCost).GetField("_card", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void Postfix(CardEnergyCost __instance, bool __result)
    {
        if (!__result) return;
        var card = CardField.GetValue(__instance) as CardModel;
        if (card?.Tags.Contains(HuoreimuTags.UltimateSkill) == true)
            card.RemoveKeyword(HuoreimuKeywords.FreeUltimateSkill);
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.AfterCardPlayedCleanup))]
public static class FreeUltimatePlayedCleanupPatch
{
    private static readonly FieldInfo CardField = typeof(CardEnergyCost).GetField("_card", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void Postfix(CardEnergyCost __instance, bool __result)
    {
        if (!__result) return;
        var card = CardField.GetValue(__instance) as CardModel;
        if (card?.Tags.Contains(HuoreimuTags.UltimateSkill) == true)
            card.RemoveKeyword(HuoreimuKeywords.FreeUltimateSkill);
    }
}
