using Godot;
using MegaCrit.Sts2.Core.Saves;

namespace AyaMod.Huoreimu.Audio;

public class HuoreimuAudio
{
    private static readonly Dictionary<string, AudioStream> Cache = new();
    private static readonly Dictionary<string, List<AudioStreamPlayer>> ActivePlayers = new();

    // 记录播放结束的回调，停止播放时用于解绑（Godot 的 C# 事件无法通过 += null 解绑）
    private static readonly Dictionary<AudioStreamPlayer, Action> FinishedHandlers = new();

    public static void Play(string path, float volume, float pitch, bool loop, bool unique)
    {
        volume *= SaveManager.Instance.SettingsSave.VolumeSfx;
        if (!Cache.TryGetValue(path, out var stream))
        {
            stream = GD.Load<AudioStream>(path);
            if (stream != null)
                Cache[path] = stream;
        }

        if (stream == null)
        {
            GD.PrintErr($"Failed to load audio stream at path: {path}");
            return;
        }

        // unique：同一音效同时只保留一个播放器，复用第一个并释放多余的
        if (unique && ActivePlayers.TryGetValue(path, out var activePlayers))
        {
            // 先清理已经失效的播放器，避免复用到已释放的对象
            for (var i = activePlayers.Count - 1; i >= 0; i--)
            {
                if (GodotObject.IsInstanceValid(activePlayers[i])) continue;
                DetachFinishedHandler(activePlayers[i]);
                activePlayers.RemoveAt(i);
            }

            if (activePlayers.Count > 0)
            {
                var currentPlayer = activePlayers[0];
                foreach (var otherPlayer in activePlayers.Skip(1).ToArray())
                {
                    activePlayers.Remove(otherPlayer);
                    DetachFinishedHandler(otherPlayer);
                    otherPlayer.Stop();
                    otherPlayer.QueueFree();
                }

                currentPlayer.VolumeDb = ToDb(volume);
                currentPlayer.PitchScale = pitch;
                currentPlayer.Play();
                return;
            }

            ActivePlayers.Remove(path);
        }

        var player = new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = ToDb(volume),
            PitchScale = pitch,
            Autoplay = true,
        };

        var tree = Engine.GetMainLoop() as SceneTree;
        tree?.Root.AddChild(player);

        // 登记播放器以便停止
        if (!ActivePlayers.TryGetValue(path, out var players))
        {
            players = [];
            ActivePlayers[path] = players;
        }

        players.Add(player);

        Action onFinished = () =>
        {
            if (loop)
            {
                // 循环播放：每次播放结束后重新播放
                if (GodotObject.IsInstanceValid(player) && !player.Playing)
                    player.Play();
                return;
            }

            // 非循环：播放完毕后自动清理
            if (ActivePlayers.TryGetValue(path, out var list))
            {
                list.Remove(player);
                if (list.Count == 0)
                    ActivePlayers.Remove(path);
            }

            DetachFinishedHandler(player);
            if (GodotObject.IsInstanceValid(player))
                player.QueueFree();
        };

        FinishedHandlers[player] = onFinished;
        player.Finished += onFinished;
    }

    private static void DetachFinishedHandler(AudioStreamPlayer player)
    {
        if (FinishedHandlers.Remove(player, out var handler))
            player.Finished -= handler;
    }

    // 立即停止
    public static void Stop(string path)
    {
        StopInternal(path, 0f);
    }

    // 淡出停止
    public static void Stop(string path, float fadeDuration)
    {
        StopInternal(path, fadeDuration);
    }

    private static void StopInternal(string path, float fadeDuration)
    {
        if (!ActivePlayers.TryGetValue(path, out var list))
            return;

        foreach (var player in list.ToList())
        {
            // 解绑播放结束回调，避免循环播放被重新触发
            DetachFinishedHandler(player);

            if (fadeDuration > 0f)
            {
                var tween = player.CreateTween();
                tween.TweenMethod(
                    Callable.From<float>(v => player.VolumeDb = v),
                    player.VolumeDb,
                    -80f,
                    fadeDuration
                );
                tween.TweenCallback(
                    Callable.From(() =>
                    {
                        player.Stop();
                        player.QueueFree();
                    })
                );
            }
            else
            {
                player.Stop();
                player.QueueFree();
            }
        }

        ActivePlayers.Remove(path);
    }

    public static void StopAll(float fadeDuration = 0f)
    {
        // 先取得所有正在播放的路径，避免遍历时修改字典
        var paths = ActivePlayers.Keys.ToList();
        foreach (var path in paths)
        {
            Stop(path, fadeDuration);
        }
    }

    private static float ToDb(float volume)
    {
        if (volume <= 0f)
            return -80f;
        return Mathf.LinearToDb(volume);
    }
}