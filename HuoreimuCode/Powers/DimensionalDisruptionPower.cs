using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public sealed class DimensionalDisruptionPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(0)];

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature applier,
        CardModel cardSource)
    {
        if (power != this) return Task.CompletedTask;
        // ReSharper disable once PossibleLossOfFraction
        DynamicVars.Energy.BaseValue = Amount / 2;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        if (Amount <= 0) return;

        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, player);
        var energy = DynamicVars.Energy.IntValue;
        if (energy > 0) await PlayerCmd.GainEnergy(energy, player);
        await PowerCmd.Remove(this);
    }
}