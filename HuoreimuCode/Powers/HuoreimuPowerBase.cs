using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;

namespace AyaMod.Huoreimu.Powers;

public abstract class HuoreimuPowerBase : CustomPowerModel
{
    protected HuoreimuPowerBase(PowerType type, PowerStackType stackType)
    {
        Type = type;
        StackType = stackType;
        _imageName = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}";
    }

    private readonly string _imageName;
    public override PowerType Type { get; }

    public override PowerStackType StackType { get; }

    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{_imageName}.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power.png".PowerIconPath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{_imageName}_big.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power_big.png".PowerIconPath();
        }
    }
}
