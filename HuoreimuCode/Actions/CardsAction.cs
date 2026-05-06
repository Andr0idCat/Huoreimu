using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Actions;

public static class CardsAction
{
    /// <summary>
    /// 将指定类型的卡牌加入牌堆。
    /// </summary>
    /// <param name="owner">卡牌的拥有者</param>
    /// <param name="pileType">牌堆类型</param>
    /// <param name="amount">加入卡牌的数量</param>
    /// <param name="animationDuration">卡牌飞入牌组的动画时间</param>
    /// <typeparam name="T">需要加入的卡牌类型</typeparam>
    public static async Task AddCardToPile<T>(Player owner, PileType pileType, int amount, float animationDuration = 2f)
        where T : CardModel
    {
        var results = new List<CardPileAddResult>();
        for (var i = 0; i < amount; i++)
        {
            CardModel card = ModelDb.Card<T>();
            results.Add(await CardPileCmd.Add(owner.RunState.CreateCard(card, owner), pileType));
        }

        CardCmd.PreviewCardPileAdd(results, animationDuration);
    }
}