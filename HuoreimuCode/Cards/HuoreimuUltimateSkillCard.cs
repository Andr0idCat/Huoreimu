using AyaMod.Huoreimu.Actions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace AyaMod.Huoreimu.Cards;

public abstract class HuoreimuUltimateSkillCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : HuoreimuCard(cost, type, rarity, target)
{
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies?.Any(e => UltimateSkillAction.CanCast(this, e)) ?? false;
}