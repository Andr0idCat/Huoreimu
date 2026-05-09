#nullable enable
using AyaMod.Huoreimu.Actions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace AyaMod.Huoreimu.Powers;

// todo: 以下的方法似乎没有覆盖完整的生命周期。可能需要仔细检查所有的情况，并确保在不需要时正确地取消订阅事件。

/// <summary>
/// 一个抽象的Power类，专门用于响应必杀技发动事件。
/// 当这个Power被应用到一个生物上时，它会订阅UltimateSkillAction.OnUltimateSkillCast事件，并在事件触发时调用OnUltimateSkillCast方法。
/// 这个方法由具体的子类实现，以定义在必杀技发动时应该执行的效果。
/// 当这个Power被移除或者战斗结束时，它会取消订阅事件，以避免潜在的内存泄漏或错误调用。
/// </summary>
/// <param name="type">能力类型</param>
/// <param name="stackType">堆叠类型</param>
public abstract class UltimateSkillCastPower(PowerType type, PowerStackType stackType)
    : HuoreimuPowerBase(type, stackType)
{
    protected abstract Task OnUltimateSkillCast(CardModel card, Creature? target, int consumed);

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UltimateSkillAction.OnUltimateSkillCast += OnUltimateSkillCast;
        Log.Info($"Applied {GetType().Name} to {Owner.Name}, subscribed to OnUltimateSkillCast");
        return base.AfterApplied(applier, cardSource);
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        UltimateSkillAction.OnUltimateSkillCast -= OnUltimateSkillCast;
        Log.Info($"Removed {GetType().Name} from {oldOwner.Name}, unsubscribed from OnUltimateSkillCast");
        return base.AfterRemoved(oldOwner);
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        UltimateSkillAction.OnUltimateSkillCast -= OnUltimateSkillCast;
        Log.Info($"Combat ended, unsubscribed {GetType().Name} from OnUltimateSkillCast");
        return base.AfterCombatEnd(room);
    }
}