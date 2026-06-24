using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AyaMod.Huoreimu.Powers;

public class GodTierFormPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        ArgumentNullException.ThrowIfNull(player);
        var prompt = SelectionScreenPrompt;
        prompt.Add(HuoreimuConstants.MAXIMUM_VALUE, Amount);
        var prefs = new CardSelectorPrefs(prompt, 0, Amount);
        var cards = PileType.Exhaust.GetPile(player).Cards;
        var selections = await CardSelectCmd.FromSimpleGrid(choiceContext, cards, player, prefs);
        await CardPileCmd.Add(selections, PileType.Hand);
    }
}