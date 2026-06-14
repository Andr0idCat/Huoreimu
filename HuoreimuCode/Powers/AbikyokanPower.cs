using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Cards;
using AyaMod.Huoreimu.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;

namespace AyaMod.Huoreimu.Powers;

public class AbikyokanPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Huo>()];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side is not CombatSide.Player) return;
        var player = Owner.Player;
        if (player is null)
        {
            Log.Warn($"[{nameof(AbikyokanPower)}] Owner.Player is null. This should not happen.");
            return;
        }

        Flash();
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), Amount, player);
        await CardsAction.AddCardToPileWhenCombat<Huo>(player, PileType.Hand, Amount, combatState);
    }

    public override string CustomPackedIconPath
    {
        get
        {
            var path = "abikyokan_power.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return path;
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = "abikyokan_power_big.png".PowerIconPath();
            Log.Info($">>>[HuoreimuMod]PowerIconPath={path}", 2);
            return path;
        }
    }
}