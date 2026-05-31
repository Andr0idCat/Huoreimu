using AyaMod.Huoreimu.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeDamageReceived))]
public static class DamageReceiveListenerPatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task __result, PlayerChoiceContext choiceContext, IRunState runState,
        CombatState combatState, Creature target, decimal amount, ValueProp props, Creature dealer,
        CardModel cardSource)
    {
        await __result;

        var player = target.Player;
        if (player?.PlayerCombatState == null) return;

        foreach (var c in player.PlayerCombatState.AllCards.Where(c => c is IDamageReceiveListener))
        {
            await c.BeforeDamageReceived(choiceContext, target, amount, props, dealer, cardSource);
        }
    }
}