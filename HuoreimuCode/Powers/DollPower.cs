using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public class DollPower : DollPowerBase
{
	protected override string IconAssetName => "doll_power";

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new(HuoreimuConstants.AUTO_PLAY_CARD_COST_MIN, 1)
	];

	protected override bool CanAutoPlay(CardModel card)
	{
		return card.EnergyCost.Canonical == 1;
	}
}
