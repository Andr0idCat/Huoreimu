using System.Collections.Concurrent;
using System.Text.Json;
using AyaMod.Huoreimu.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace AyaMod.Huoreimu.Utils;

#nullable enable
public static class VfxUtil
{
    // Mod 独立的场景缓存（避免被 PreloadManager 清理）
    private static readonly ConcurrentDictionary<string, PackedScene> ModSceneCache = new();

    public static void LoadVfx()
    {
        using var file = Godot.FileAccess.Open("vfx_manifest.json".VFXPath(), Godot.FileAccess.ModeFlags.Read);
        if (file == null) return;

        var json = file.GetAsText();
        var relativePaths = JsonSerializer.Deserialize<List<string>>(json);
        if (relativePaths == null) return;

        foreach (var relativePath in relativePaths)
        {
            var fullPath = relativePath.VFXPath();
            if (ModSceneCache.ContainsKey(fullPath)) continue;
            var scene = ResourceLoader.Load<PackedScene>(fullPath);
            if (scene != null)
            {
                ModSceneCache[fullPath] = scene;
            }
        }
    }

    public static Node2D GenVfxNode(string scenePath)
    {
        return ModSceneCache.TryGetValue(scenePath, out var modScene)
            ? modScene.Instantiate<Node2D>()
            : PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>();
    }

    public static T GenVfxNode<T>(string scenePath) where T : Node2D
    {
        return ModSceneCache.TryGetValue(scenePath, out var modScene)
            ? modScene.Instantiate<T>()
            : PreloadManager.Cache.GetScene(scenePath).Instantiate<T>();
    }

    /// <summary>
    /// 在指定目标身上播放特效
    /// </summary>
    /// <param name="vfxName">特效名称</param>
    /// <param name="target">目标</param>
    /// <param name="lifetime">特效时间（超过时间后将特效销毁）</param>
    /// <param name="needFlip">特效是否翻转（被夹击时攻击身后的目标，可能需要将特效翻转）</param>
    /// <returns></returns>
    public static Node2D? PlaySimple(string vfxName, Creature target, float lifetime = 2f, bool needFlip = true)
    {
        if (TestMode.IsOn || NCombatRoom.Instance == null) return null;
        var node2D = GenVfxNode($"{vfxName}.tscn".VFXPath());

        var parent = NCombatRoom.Instance.CombatVfxContainer;
        var ownerNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (ownerNode is null) return null;
        var spawnPos = ownerNode.VfxSpawnPosition;
        parent.AddChildSafely(node2D);
        node2D.GlobalPosition = spawnPos;

        if (needFlip)
        {
            node2D.Scale = new Vector2(-node2D.Scale.X, node2D.Scale.Y);
        }

        // 创建定时器，超时后销毁
        var timer = node2D.GetTree().CreateTimer(lifetime);
        timer.Timeout += () =>
        {
            if (GodotObject.IsInstanceValid(node2D))
            {
                node2D.QueueFreeSafely();
            }
        };
        return node2D;
    }

    public static bool NeedFlip(Creature creature)
    {
        var body = NCombatRoom.Instance?.GetCreatureNode(creature)?.Body;
        if (body == null) return false;
        return body.Scale.X < 0;
    }
}