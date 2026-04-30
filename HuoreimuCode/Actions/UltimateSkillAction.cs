using AyaMod.Huoreimu.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Actions;

public static class UltimateSkillAction
{
    public static async Task Execute(CardModel card, CardPlay cardPlay, Func<Task> action)
    {
        if (!card.DynamicVars.TryGetValue(HuoreimuConstants.ULTIMATE_SKILL, out var dynamicVar))
        {
            return;
        }

        if (cardPlay?.Target?.Powers?.FirstOrDefault(p => p is OmenOfDeathPower) is not OmenOfDeathPower omen ||
            omen.Amount < dynamicVar.IntValue)
        {
            return;
        }

        await PowerCmd.ModifyAmount(omen, -dynamicVar.IntValue, card.Owner.Creature, card);
        var task = action?.Invoke();
        if (task is null) return;
        await task;
    }
}