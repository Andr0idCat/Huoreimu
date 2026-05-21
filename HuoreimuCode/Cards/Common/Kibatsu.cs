using System.Linq;
using AyaMod.Huoreimu.Actions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class Kibatsu() : HuoreimuCard(4, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
	private const int MAX_ENERGY_COST = 4;
	private const int DECREASE_COST = 1;

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new DamageVar(24, ValueProp.Move),
		new(HuoreimuConstants.DECREASE_VALUE, DECREASE_COST)
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target);
		await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(MAX_ENERGY_COST);
	}

	public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)
	{
		if (CombatState == null || Pile?.Type != PileType.Hand) return Task.CompletedTask;

		var state = Owner.PlayerCombatState;
		ArgumentNullException.ThrowIfNull(state);
		var curseCount = state.AllCards.Count(c => c.Type == CardType.Curse);
		EnergyCost.SetThisTurn(Math.Max(0, MAX_ENERGY_COST - curseCount * DECREASE_COST));
		CardsAction.Refresh(this);
		return Task.CompletedTask;
	}
}
