using System.Collections.Generic;
using System.Threading.Tasks;
using AyaMod.Huoreimu.Character;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Relic;

public class BlackGohei : HuoreimuRelic
{
	public override RelicRarity Rarity => RelicRarity.Rare;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented,
		float deathAnimLength)
	{
		if (target.Side == Owner.Creature.Side) return;

		Flash();
		await PowerCmd.Apply<IntangiblePower>(Owner.Creature, 1, Owner.Creature, null);
	}
}
