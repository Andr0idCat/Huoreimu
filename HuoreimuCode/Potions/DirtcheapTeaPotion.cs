using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AyaMod.Huoreimu.Potions;

public class DirtcheapTeaPotion() : HuoreimuPotion(PotionRarity.Token, PotionUsage.CombatOnly, TargetType.AnyPlayer)
{
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature target)
    {
        var player = target?.Player;
        ArgumentNullException.ThrowIfNull(player);
        await CardPileCmd.Draw(choiceContext, 1, player);
        var statusCard = PileType.Hand.GetPile(player).Cards
            .Where(c => c.Type is CardType.Status)
            .ToList();
        if (statusCard.Count == 0) return;
        var rng = player.RunState.Rng.Niche;
        var selection = rng.NextItem(statusCard);
        await CardCmd.Exhaust(choiceContext, selection);
    }
}