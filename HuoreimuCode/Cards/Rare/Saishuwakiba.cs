using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Cards.Rare;

public class Saishuwakiba() : HuoreimuCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OmenOfDeathPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OmenOfDeathPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var hittableEnemy in CombatState.HittableEnemies)
        {
            await CommonActions.Apply<OmenOfDeathPower>(choiceContext, hittableEnemy, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OmenOfDeathPower>().UpgradeValueBy(2);
    }
}
