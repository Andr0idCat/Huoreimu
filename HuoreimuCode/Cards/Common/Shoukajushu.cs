using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Extensions;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Tags;
using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Common;

public class Shoukajushu() : HuoreimuUltimateSkillCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new UltimateSkillVar(3),
        new PowerVar<ShoukajushuPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await UltimateSkillAction.Execute(choiceContext, this, cardPlay, async () =>
        {
            await CommonActions.ApplySelf<ShoukajushuPower>(choiceContext, this);
        });
    }

    protected override void OnUpgrade()
    {
        var ultimateSkill = DynamicVars.GetUltimateSkill();
        if (ultimateSkill != null) ultimateSkill.UpgradeValueBy(-1);
    }
}