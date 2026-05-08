using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.DynamicVars;

public class UltimateSkillVar : DynamicVar
{
    public bool CostAll { get; }

    private UltimateSkillVar(int value, bool costAll) : base(HuoreimuConstants.ULTIMATE_SKILL, value)
    {
        CostAll = costAll;
    }

    /// <summary>
    /// 需要消耗一定层数的死兆星来发动的必杀技。
    /// </summary>
    /// <param name="value">必杀技发动需要的死兆星层数</param>
    public UltimateSkillVar(int value) : this(value, false)
    {
    }

    /// <summary>
    /// 消耗所有死兆星来发动的必杀技。发动条件是目标至少拥有1层死兆星。
    /// </summary>
    public UltimateSkillVar() : this(1, true)
    {
    }
}