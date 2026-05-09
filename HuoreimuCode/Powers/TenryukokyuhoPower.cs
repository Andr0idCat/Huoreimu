using AyaMod.Huoreimu.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Powers;

public class TenryukokyuhoPower() : UltimateSkillCastPower(PowerType.Buff, PowerStackType.Counter)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override Task OnUltimateSkillCast(CardModel card, Creature target, int consumed)
    {
        return PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}