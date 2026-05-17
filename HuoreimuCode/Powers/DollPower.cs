using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public sealed class DollPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override bool IsInstanced => true;
    private int _autoPlayCardCost = 1;

    public int AutoPlayCardCost
    {
        get => _autoPlayCardCost;
        set
        {
            _autoPlayCardCost = value;
            if (!DynamicVars.TryGetValue(HuoreimuConstants.AUTO_PLAY_CARD_COST, out var dynamicVar)) return;
            dynamicVar.BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HuoreimuConstants.AUTO_PLAY_CARD_COST, AutoPlayCardCost)
    ];

    public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.Player is null)
        {
            return;
        }

        var hand = PileType.Draw.GetPile(Owner.Player);
        var items = hand.Cards
            .Where(c => c.Type == CardType.Attack &&
                        !c.Keywords.Contains(CardKeyword.Unplayable) &&
                        c.EnergyCost.Canonical == AutoPlayCardCost)
            .ToList();
        var cardModel = Owner.Player.RunState.Rng.Shuffle.NextItem(items);
        if (cardModel == null) return;
        Flash();
        await CardCmd.AutoPlay(choiceContext, cardModel, null);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0) return;

        Flash();
        await PowerCmd.ModifyAmount(this, -result.UnblockedDamage, target, cardSource);
    }
}