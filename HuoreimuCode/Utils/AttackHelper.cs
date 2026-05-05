using System.Reflection;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Utils;

public static class AttackHelper
{
    private static readonly PropertyInfo DamagePropsProperty =
        typeof(AttackCommand).GetProperty("DamageProps", BindingFlags.Public | BindingFlags.Instance)!;

    public static AttackCommand WithProp(this AttackCommand command, ValueProp prop)
    {
        DamagePropsProperty.SetValue(command, command.DamageProps | prop);
        return command;
    }
}