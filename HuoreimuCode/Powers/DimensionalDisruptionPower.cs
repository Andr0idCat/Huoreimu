using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AyaMod.Huoreimu.Powers;

public sealed class DimensionalDisruptionPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        if (Amount <= 0) return;

        Flash();

        // Per card discarded: draw 1
        await CardPileCmd.Draw(choiceContext, Amount, player);

        // Per 2 cards discarded: gain 1 energy
        var energy = Amount / 2;
        if (energy > 0) await PlayerCmd.GainEnergy(energy, player);

        await PowerCmd.Remove(this);
    }
}
