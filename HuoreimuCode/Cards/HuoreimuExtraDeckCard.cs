using AyaMod.Huoreimu.Actions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Cards;

/// <summary>
/// 将卡牌加入牌堆的卡牌基类。当这张牌被加入牌堆时，会额外将指定类型的卡牌加入牌堆。
/// </summary>
/// <param name="cost">卡牌费用</param>
/// <param name="type">卡牌类型（攻击、技能、能力）</param>
/// <param name="rarity">卡牌稀有度（普通、罕见、稀有）</param>
/// <param name="target">卡牌目标</param>
/// <param name="amount">额外加入牌堆的卡牌数量，默认为1</param>
/// <typeparam name="T">额外加入牌堆的卡牌类型</typeparam>
public abstract class HuoreimuExtraDeckCard<T>(int cost, CardType type, CardRarity rarity, TargetType target, int amount = 1)
    : HuoreimuCard(cost, type, rarity, target) where T : CardModel
{
    public override async Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel source)
    {
        var pile = card.Pile;
        if (pile is not { Type: PileType.Deck } || card != this)
        {
            return;
        }

        await CardsAction.AddCardToPile<T>(Owner, PileType.Deck, amount);
    }
}