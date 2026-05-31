using AyaMod.Huoreimu.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Extensions;

public static class DynamicVarsExtension
{
    public static UltimateSkillVar GetUltimateSkill(this DynamicVarSet set)
    {
        return set.TryGetValue(HuoreimuConstants.ULTIMATE_SKILL, out var dynamicVar)
            ? dynamicVar as UltimateSkillVar
            : null;
    }

    public static PercentVar GetPercent(this DynamicVarSet set)
    {
        return set.TryGetValue(HuoreimuConstants.PERCENT_VALUE, out var dynamicVar) ? dynamicVar as PercentVar : null;
    }

    public static SelectionVar GetSelection(this DynamicVarSet set)
    {
        return set.TryGetValue(HuoreimuConstants.SELECTION_COUNT, out var dynamicVar)
            ? dynamicVar as SelectionVar
            : null;
    }
}