using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.DynamicVars;

public static class DynamicVarsExtension
{
    public static PercentVar GetPercent(this DynamicVarSet set)
    {
        return set.TryGetValue(HuoreimuConstants.PERCENT_VALUE, out var dynamicVar) ? dynamicVar as PercentVar : null;
    }
}