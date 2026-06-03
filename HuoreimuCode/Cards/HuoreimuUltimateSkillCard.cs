using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Cards;

public abstract class HuoreimuUltimateSkillCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : HuoreimuCard(cost, type, rarity, target)
{
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies?.Any(e => UltimateSkillAction.CanCast(this, e)) ?? false;

    protected static int CalculateXUltimateValue(CardModel card, Creature creature)
    {
        return card.CombatState is null
            ? 0
            : Hook.ModifyXValue(card.CombatState, card, creature?.GetPower<OmenOfDeathPower>()?.Amount ?? 0);
    }
}