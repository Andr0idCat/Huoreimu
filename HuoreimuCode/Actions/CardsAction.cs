using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AyaMod.Huoreimu.Actions;

public static class CardsAction
{
    /// <summary>
    /// 将指定类型的卡牌加入卡组。
    /// </summary>
    /// <param name="owner">卡牌的拥有者</param>
    /// <param name="amount">加入卡牌的数量</param>
    /// <param name="animationDuration">卡牌飞入牌组的动画时间</param>
    /// <typeparam name="T">需要加入的卡牌类型</typeparam>
    public static async Task AddCardToDeck<T>(Player owner, int amount, float animationDuration = 2f)
        where T : CardModel
    {
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < amount; i++)
        {
            CardModel card = ModelDb.Card<T>();
            results.Add(await CardPileCmd.Add(owner.RunState.CreateCard(card, owner), PileType.Deck));
        }

        CardCmd.PreviewCardPileAdd(results, animationDuration);
    }

    /// <summary>
    /// 在战斗中将指定类型的卡牌加入指定牌堆。
    /// </summary>
    /// <param name="owner">卡牌的拥有者</param>
    /// <param name="pileType">牌堆类型</param>
    /// <param name="amount">加入卡牌的数量</param>
    /// <param name="combatState">战斗状态</param>
    /// <param name="position">卡牌在牌堆中的位置</param>
    /// <typeparam name="T">需要加入的卡牌类型</typeparam>
    /// <returns></returns>
    public static async Task<IEnumerable<CardModel>> AddCardToPileWhenCombat<T>(Player owner, PileType pileType,
        int amount, CombatState combatState, CardPilePosition position = CardPilePosition.Bottom) where T : CardModel
    {
        if (amount == 0 || CombatManager.Instance.IsOverOrEnding)
        {
            return [];
        }

        var cards = new List<CardModel>();
        for (var i = 0; i < amount; i++)
        {
            cards.Add(combatState.CreateCard<T>(owner));
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, pileType, true, position);
        return cards;
    }

    /// <summary>
    /// 刷新卡牌的视觉效果，通常在卡牌属性发生变化时调用。
    /// </summary>
    /// <param name="card">要刷新的卡牌</param>
    /// <param name="mode">卡牌预览模式，默认为正常模式</param>
    public static void Refresh(CardModel card, CardPreviewMode mode = CardPreviewMode.Normal)
    {
        var pile = card.Pile;
        if (pile == null) return;
        NCard.FindOnTable(card)?.UpdateVisuals(pile.Type, mode);
    }
}