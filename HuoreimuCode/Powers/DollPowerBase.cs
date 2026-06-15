using AyaMod.Huoreimu.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Powers;

public abstract class DollPowerBase() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected abstract string IconAssetName { get; }

    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{IconAssetName}.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power.png".PowerIconPath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{IconAssetName}_big.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return ResourceLoader.Exists(path) ? path : "empty_power_big.png".PowerIconPath();
        }
    }

    protected abstract bool CanAutoPlay(CardModel card);

#nullable enable
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
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
        await PowerCmd.ModifyAmount(choiceContext, this, -result.UnblockedDamage, target, cardSource, false);
    }
}
