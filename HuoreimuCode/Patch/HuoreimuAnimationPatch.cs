using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using AyaMod.Huoreimu.Character;
using static Godot.Node;
using static Godot.PackedScene;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class HuoreimuAnimationPatch
{
	// 角色模型的 ModelId，由 Mod 前缀(AYAMOD) + 角色 Id(Huoreimu) 组成
	private const string CharacterModelId = "CHARACTER.AYAMOD-HUOREIMU";

	// 标记已经挂过"动画结束回到 Idle"回调的节点，避免重复连接
	private const string IdleReturnConnectedMeta = "huoreimu_idle_return_connected";

	public static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Entity == null || !__instance.Entity.IsPlayer)
			return;

		if (__instance.Entity.ModelId.ToString() == CharacterModelId)
		{
			Log.Info($"[Huoreimu] AnimeTrigger={trigger}");
			switch (trigger)
			{
				case "Hit":
					PlayAnim(__instance, "Hit", false);
					break;

				case "Attack":
					PlayAnim(__instance, GetRandomAttackAnim(), false);
					break;
				
				case "Cast":
					PlayAnim(__instance, "Cast", false);
					break;

				case "Dead":
					PlayAnim(__instance, "Dead", false);
					break;

				default:
					PlayAnim(__instance, "Idle", false);
					break;
			}
		}
	}

	private static string GetRandomAttackAnim()
	{
		var attacks = new[] { "Attack", "Attack2", "Attack3", "Attack4" };
		var index = (int)(GD.Randf() * attacks.Length);
		return attacks[Math.Clamp(index, 0, attacks.Length - 1)];
	}

	private static void PlayAnim(NCreature node, string animName,bool fromEnd)
	{
		var visual = node.GetNodeOrNull<Node2D>("Huoreimu");
		if (visual == null) return;

		var anim = visual.GetNodeOrNull<AnimatedSprite2D>("HuoreimuVisual");
		if (anim == null) return;

		// 每个节点只连接一次，避免动画播放时不断累积回调
		if (!anim.HasMeta(IdleReturnConnectedMeta))
		{
			anim.SetMeta(IdleReturnConnectedMeta, true);
			anim.AnimationFinished += () =>
			{
				if (!GodotObject.IsInstanceValid(anim)) return;
				if (anim.Animation == "Dead" || anim.Animation == "Idle") return;
				anim.Play("Idle");
			};
		}

		// 切换动画
		anim.Frame = 0;
		anim.Play(animName, 1f, fromEnd);
	}
}

[HarmonyPatch(typeof(NGameOverScreen), "MoveCreaturesToDifferentLayerAndDisableUi")]
public static class HuoreimuGameOverPatch
{
	public static void Postfix(NGameOverScreen __instance)
	{
		var creatureContainer = __instance.GetNodeOrNull<Control>("%CreatureContainer");
		if (creatureContainer == null) return;

		foreach (var child in creatureContainer.GetChildren())
		{
			if (child is not Node2D visual) continue;
			if (!visual.Name.ToString().StartsWith("Huoreimu")) continue;

			var anim = visual.GetNodeOrNull<AnimatedSprite2D>("HuoreimuVisual");
			if (anim == null) continue;
			anim.Play("Dead");
		}
	}
}
