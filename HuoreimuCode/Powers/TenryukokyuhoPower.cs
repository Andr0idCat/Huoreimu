using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Powers;

public class TenryukokyuhoPower() : HuoreimuPowerBase(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (!card.Tags.Contains(HuoreimuTags.UltimateSkill) || !UltimateSkillAction.CanCast(card, cardPlay.Target))
            return Task.CompletedTask;
        return PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}