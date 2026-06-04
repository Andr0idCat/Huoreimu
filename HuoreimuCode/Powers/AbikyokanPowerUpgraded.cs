using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public class AbikyokanPowerUpgraded() : AbikyokanPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Huo>()];

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature applier,
        CardModel cardSource)
    {
        if (power == this)
        {
            DynamicVars.Cards.BaseValue = Amount * 2;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side is not CombatSide.Player) return;
        var player = Owner.Player;
        if (player is null)
        {
            Log.Warn($"[{nameof(AbikyokanPower)}] Owner.Player is null. This should not happen.");
            return;
        }

        Flash();
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Amount * 2, player);
        await CardsAction.AddCardToPileWhenCombat<Huo>(player, PileType.Hand, Amount, combatState);
    }
}