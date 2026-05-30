using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.DynamicVars;

public class SelectionVar : DynamicVar
{
    public SelectionVar(decimal value) : base(HuoreimuConstants.SELECTION_COUNT, value)
    {
    }
}