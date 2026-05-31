using AyaMod.Huoreimu.Cards;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Actions;

public static class UltimateSkillAction
{
    private static bool IsFree(CardModel card) => card.Keywords.Contains(HuoreimuKeywords.FreeUltimateSkill);

    private static (OmenOfDeathPower, int) CheckCondition(CardModel card, Creature creature)
    {
        if (!card.DynamicVars.TryGetValue(HuoreimuConstants.ULTIMATE_SKILL, out var dynamicVar))
        {
            dynamicVar = card.Enchantment?.DynamicVars?.GetUltimateSkill();
            if (dynamicVar is null) return (null, 0);
        }

        var omen = creature?.GetPower<OmenOfDeathPower>();
        if (IsFree(card))
        {
            return (omen, dynamicVar.IntValue);
        }

        if (omen is null || omen.Amount < dynamicVar.IntValue)
        {
            return (null, 0);
        }

        return (omen, dynamicVar.IntValue);
    }

    private static async Task<bool> InternalExecute(CardModel card, CardPlay cardPlay)
    {
        var (omen, value) = CheckCondition(card, cardPlay.Target);
        var cast = CanCast(card, omen, value);
        if (!cast) return false;
        if (card is HuoreimuCard huoreimuCard) huoreimuCard.PlayUltimateSkillAudio();
        if (!IsFree(card)) await PowerCmd.ModifyAmount(omen, -value, card.Owner.Creature, card);
        return true;
    }

    private static bool CanCast(CardModel card, OmenOfDeathPower omen, int value)
    {
        if (IsFree(card)) return true;
        return omen != null && omen.Amount >= value;
    }

    /// <summary>
    /// 检查目标是否满足必杀技的发动条件（即拥有足够的死兆星）。如果满足条件，返回true；否则返回false。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="target">目标</param>
    /// <returns></returns>
    public static bool CanCast(CardModel card, Creature target)
    {
        var (omen, value) = CheckCondition(card, target);
        return CanCast(card, omen, value);
    }

    /// <summary>
    /// 如果目标满足必杀技的发动条件，则消耗对应的死兆星并执行后续动作。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="cardPlay">CardPlay</param>
    /// <param name="onCast">如果必杀技可以发动，则执行其返回的Task</param>
    public static async Task Execute(CardModel card, CardPlay cardPlay, Func<Task> onCast)
    {
        var cast = await InternalExecute(card, cardPlay);
        if (!cast) return;
        var task = onCast?.Invoke();
        if (task is null)
        {
            Log.Warn($"Ultimate skill {card.Title} was cast, but no follow-up action was provided.");
            return;
        }

        await task;
    }

    /// <summary>
    /// 如果目标满足必杀技的发动条件，则消耗对应的死兆星并执行onCast；否则执行onFallback。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="cardPlay">CardPlay</param>
    /// <param name="onCast">如果必杀技可以发动，则执行其返回的Task</param>
    /// <param name="onFallback">如果必杀技没有满足发动条件，则执行其返回的Task</param>
    public static async Task Execute(CardModel card, CardPlay cardPlay, Func<Task> onCast, Func<Task> onFallback)
    {
        var cast = await InternalExecute(card, cardPlay);
        var task = cast ? onCast?.Invoke() : onFallback?.Invoke();
        if (task is null)
        {
            Log.Warn($"Ultimate skill {card.Title} was cast, but no follow-up action was provided.");
            return;
        }

        await task;
    }

    /// <summary>
    /// 如果目标满足必杀技的发动条件，则先执行动作，再消耗对应的死兆星；否则不执行任何操作。避免先消耗死兆星后，伤害计算有误。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="cardPlay">CardPlay</param>
    /// <param name="onCast">如果必杀技可以发动，则执行其返回的Task</param>
    public static async Task EarlyExecute(CardModel card, CardPlay cardPlay, Func<Task> onCast)
    {
        var (omen, value) = CheckCondition(card, cardPlay.Target);
        var cast = CanCast(card, omen, value);
        if (!cast) return;
        if (card is HuoreimuCard huoreimuCard) huoreimuCard.PlayUltimateSkillAudio();
        var task = onCast?.Invoke();
        if (task is null)
        {
            Log.Warn($"Ultimate skill {card.Title} was cast, but no follow-up action was provided.");
        }
        else
        {
            await task;
        }

        if (!IsFree(card)) await PowerCmd.ModifyAmount(omen, -value, card.Owner.Creature, card);
    }

    /// <summary>
    /// 如果目标满足必杀技的发动条件，则先执行动作，再消耗对应的死兆星；否则不执行任何操作。避免先消耗死兆星后，伤害计算有误。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="cardPlay">CardPlay</param>
    /// <param name="onCast">如果必杀技可以发动，则执行其返回的Task</param>
    /// <param name="onFallback">如果必杀技没有满足发动条件，则执行其返回的Task</param>
    public static async Task EarlyExecute(CardModel card, CardPlay cardPlay, Func<Task> onCast,
        Func<Task> onFallback)
    {
        var (omen, value) = CheckCondition(card, cardPlay.Target);
        var cast = CanCast(card, omen, value);
        if (card is HuoreimuCard huoreimuCard) huoreimuCard.PlayUltimateSkillAudio();
        var task = cast ? onCast?.Invoke() : onFallback?.Invoke();
        if (task is null)
        {
            Log.Warn($"Ultimate skill {card.Title} was cast, but no follow-up action was provided.");
        }
        else
        {
            await task;
        }

        if (cast && !IsFree(card)) await PowerCmd.ModifyAmount(omen, -value, card.Owner.Creature, card);
    }

    /// <summary>
    /// 消耗目标所有死兆星，每消耗一层执行一次onCast。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="cardPlay">CardPlay</param>
    /// <param name="onCast">接收消耗层数的回调</param>
    public static async Task ExecuteAll(CardModel card, CardPlay cardPlay, Func<int, Task> onCast)
    {
        var omen = cardPlay.Target?.GetPower<OmenOfDeathPower>();
        var count = omen?.Amount ?? 0;
        if (card is HuoreimuCard huoreimuCard) huoreimuCard.PlayUltimateSkillAudio();
        if (count > 0 && omen is not null && !IsFree(card))
            await PowerCmd.ModifyAmount(omen, -count, card.Owner.Creature, card);
        await onCast(count);
    }

    /// <summary>
    /// 先执行onCast，再消耗目标所有死兆星。避免先消耗死兆星后，伤害计算有误。
    /// </summary>
    /// <param name="card">必杀技卡牌</param>
    /// <param name="cardPlay">CardPlay</param>
    /// <param name="onCast">接收消耗层数的回调</param>
    public static async Task EarlyExecuteAll(CardModel card, CardPlay cardPlay, Func<int, Task> onCast)
    {
        var omen = cardPlay.Target?.GetPower<OmenOfDeathPower>();
        var count = omen?.Amount ?? 0;
        if (card is HuoreimuCard huoreimuCard) huoreimuCard.PlayUltimateSkillAudio();
        await onCast(count);
        if (!IsFree(card))
        {
            omen ??= cardPlay.Target?.GetPower<OmenOfDeathPower>();
            if (count > 0 && omen is not null) await PowerCmd.ModifyAmount(omen, -count, card.Owner.Creature, card);
        }
    }
}