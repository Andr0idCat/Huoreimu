using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;

namespace AyaMod.Huoreimu.Powers;

public class AbikyokanPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
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
		await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Amount, player);
		await CardsAction.AddCardToPileWhenCombat<Huo>(player, PileType.Hand, Amount, combatState);
	}
}
