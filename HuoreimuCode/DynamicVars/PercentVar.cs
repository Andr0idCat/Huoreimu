using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.DynamicVars;

public class PercentVar(decimal percent) : DynamicVar(HuoreimuConstants.PERCENT_VALUE, percent)
{
    public decimal PercentValue => BaseValue / 100m;
}