using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.DynamicVars;
using AyaMod.Huoreimu.Keywords;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Rare;

public class HellsGate() : HuoreimuCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(32, ValueProp.Move),
        new UltimateSkillVar(5),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await UltimateSkillAction.Execute(this, cardPlay, () => CreatureCmd.Stun(cardPlay.Target));
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}