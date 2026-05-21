using AyaMod.Huoreimu.Character;
using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace AyaMod.Huoreimu.Relic;

[Pool(typeof(HuoreimuRelicPool))]
public abstract class HuoreimuRelic : CustomRelicModel
{
    private string Path => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    private string GetPathOrDefault(string defaultPath)
    {
        var path = Path;
        Log.Info(">>>[HuoreimuMod]RelicPath=" + path);
        return ResourceLoader.Exists(path) ? path : defaultPath;
    }

    // 小图标（原版85x85）
    public override string PackedIconPath => GetPathOrDefault(base.PackedIconPath);

    // 轮廓图标（原版85x85）
    protected override string PackedIconOutlinePath => GetPathOrDefault(base.PackedIconOutlinePath);

    // 大图标（原版256x256）
    protected override string BigIconPath => GetPathOrDefault(base.BigIconPath);
}