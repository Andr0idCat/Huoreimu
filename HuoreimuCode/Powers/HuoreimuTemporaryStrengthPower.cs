using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Powers;

public abstract class HuoreimuTemporaryStrengthPower<T> : TemporaryStrengthPower, ICustomPower where T : CardModel
{
    public override AbstractModel OriginModel => ModelDb.Card<T>();
    protected override bool IsPositive => false;
    private readonly string _imageName;

    public HuoreimuTemporaryStrengthPower()
    {
        _imageName = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}";
    }

    public virtual string CustomPackedIconPath
    {
        get
        {
            var path = $"{_imageName}.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power.png".PowerIconPath();
        }
    }

    public virtual string CustomBigIconPath
    {
        get
        {
            var path = $"{_imageName}_big.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power_big.png".PowerIconPath();
        }
    }

    public virtual string CustomBigBetaIconPath
    {
        get
        {
            var path = $"{_imageName}_big.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power_big.png".PowerIconPath();
        }
    }
}