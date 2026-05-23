using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public abstract class DollPowerBase() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    public override bool IsInstanced => true;

    protected abstract bool CanAutoPlay(CardModel card);

#nullable enable
    public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var playable = CombatState?.HittableEnemies?.Any() ?? false;
        if (side != Owner.Side || Owner.Player is null || !playable)
        {
            return;
        }

        var hand = PileType.Draw.GetPile(Owner.Player);
        var items = hand.Cards
            .Where(c => c.Type == CardType.Attack &&
                        !c.Keywords.Contains(CardKeyword.Unplayable) &&
                        CanAutoPlay(c))
            .ToList();
        var cardModel = Owner.Player.RunState.Rng.Shuffle.NextItem(items);
        if (cardModel == null) return;
        Flash();
        await CardCmd.AutoPlay(choiceContext, cardModel, null);
    }
#nullable disable

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0) return;

        Flash();
        await PowerCmd.ModifyAmount(this, -result.UnblockedDamage, target, cardSource);
    }
}