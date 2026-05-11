using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using AyaMod.Huoreimu.Powers;
using AyaMod.Huoreimu.Tags;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Ancient;

public class Jubousuikyousatsu()
    : HuoreimuUltimateSkillCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy), ITranscendenceCard
{
    protected override HashSet<CardTag> CanonicalTags => [HuoreimuTags.UltimateSkill];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(13),
        new ExtraDamageVar(10),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((_, creature) => creature?.GetPower<OmenOfDeathPower>()?.Amount ?? 0),
        new UltimateSkillVar()
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await UltimateSkillAction.EarlyExecuteAll(this, cardPlay, _ => Attack(choiceContext, cardPlay));
    }

    private Task Attack(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }

    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<HeadbuttAttack>();
}