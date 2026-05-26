using AyaMod.Huoreimu.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards;

public class Downfall() : HuoreimuCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new DamageVar(16, ValueProp.Move | ValueProp.Unblockable)
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target);
		await CommonActions.CardAttack(this, cardPlay.Target)
			.WithProp(DynamicVars.Damage.Props)
			.Execute(choiceContext);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(6);
	}
}
