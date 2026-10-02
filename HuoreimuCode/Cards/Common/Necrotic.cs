using AyaMod.Huoreimu.Powers;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Cards.Common;

public class Necrotic() : HuoreimuCard(2, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<OmenOfDeathPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<NecroticPower>(3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(CombatState);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.AttackAnimDelay);
        foreach (var enemy in CombatState.HittableEnemies)
        {
            var omen = enemy.GetPower<OmenOfDeathPower>();
            if (omen is null || omen.Amount <= 0) continue;
            await PowerCmd.Apply<NecroticPower>(choiceContext, enemy, omen.Amount * DynamicVars.Power<NecroticPower>().IntValue,
                Owner.Creature, this, false);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}