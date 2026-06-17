#nullable enable

using AyaMod.Huoreimu.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Extensions;

public static class VfxExtension
{
    public static Node2D? PlayVfx(this CardModel card, string vfxName, Creature target, float duration = 2f,
        bool canFlip = true)
    {
        return VfxUtil.PlaySimple(vfxName, target, duration, canFlip && VfxUtil.NeedFlip(card.Owner.Creature));
    }
}