using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Enchantment;

public class MugenBondPunchEnchantment() : HuoreimuEnchantment(false, true)
{
    private bool CastUltimateSkill { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new UltimateSkillVar(3), new RepeatVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    public override bool CanEnchant(CardModel card)
    {
        return card?.TargetType is TargetType.AnyEnemy;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card == Card && UltimateSkillAction.CanCast(Card, cardPlay.Target) &&
            Status is EnchantmentStatus.Normal)
        {
            CastUltimateSkill = true;
        }

        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        Status = EnchantmentStatus.Normal;
        return base.AfterPlayerTurnStart(choiceContext, player);
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay);
        if (cardPlay.Card != Card || !CastUltimateSkill) return;
        CastUltimateSkill = false;
        Status = EnchantmentStatus.Disabled;
        await UltimateSkillAction.Execute(Card, cardPlay, () => OnCast(choiceContext, cardPlay));
    }

    private async Task OnCast(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (var i = 0; i < DynamicVars.Repeat.IntValue; ++i)
        {
            await CardCmd.AutoPlay(choiceContext, Card, cardPlay.Target);
        }
    }
}