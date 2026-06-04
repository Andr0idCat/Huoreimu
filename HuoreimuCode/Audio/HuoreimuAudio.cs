using Godot;
using MegaCrit.Sts2.Core.Saves;

namespace AyaMod.Huoreimu.Audio;

public class HuoreimuAudio
{
    private static readonly Dictionary<string, AudioStream> Cache = new();
    private static readonly Dictionary<string, List<AudioStreamPlayer>> ActivePlayers = new();

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

        AudioStreamPlayer player;
        if (unique && ActivePlayers.TryGetValue(path, out var activePlayer) && activePlayer.Count > 0)
        {
            player = activePlayer[0];
            player.Play();
            for (var i = 1; i < activePlayer.Count; i++)
            {
                var otherPlayer = activePlayer[i];
                activePlayer.Remove(otherPlayer);
                otherPlayer.Stop();
                otherPlayer.QueueFree();
            }
        }
        else
        {
            player = new AudioStreamPlayer
            {
                Stream = stream,
                VolumeDb = ToDb(volume),
                PitchScale = pitch,
                Autoplay = true,
            };

            var tree = Engine.GetMainLoop() as SceneTree;
            tree?.Root.AddChild(player);

            // 登记播放器以便停止
            if (!ActivePlayers.ContainsKey(path))
                ActivePlayers[path] = [];
            ActivePlayers[path].Add(player);
        }

        if (loop)
        {
            // 循环播放：每次播放结束后重新播放
            player.Finished += () =>
            {
                if (GodotObject.IsInstanceValid(player) && !player.Playing)
                    player.Play();
            };
        }
        else
        {
            // 非循环：播放完毕后自动清理
            player.Finished += () =>
            {
                if (ActivePlayers.TryGetValue(path, out var list))
                {
                    list.Remove(player);
                    if (list.Count == 0)
                        ActivePlayers.Remove(path);
                }

                player.QueueFree();
            };
        }
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
            // 断开所有事件，避免循环重新触发
            player.Finished += null;

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