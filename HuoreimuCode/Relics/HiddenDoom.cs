using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace AyaMod.Huoreimu.Relic;

public class HiddenDoom : HuoreimuRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool IsAllowed(IRunState runState)
    {
        return IsBeforeAct3TreasureChest(runState);
    }

    public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != Owner)
        {
            return false;
        }

        if (Owner.RunState.CurrentRoom is not { RoomType: RoomType.Elite })
        {
            return false;
        }

        foreach (var cardReward in cardRewards)
        {
            var card = cardReward.Card;
            if (!card.IsUpgradable) continue;
            var card2 = Owner.RunState.CloneCard(card);
            CardCmd.Upgrade(card2);
            cardReward.ModifyCard(card2, this);
        }

        return true;
    }
}