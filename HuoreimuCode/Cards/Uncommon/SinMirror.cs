using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace AyaMod.Huoreimu.Cards;

public class SinMirror() : HuoreimuCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var monster = cardPlay.Target?.Monster;
        ArgumentNullException.ThrowIfNull(monster);
        if (!monster.IntendsToAttack) return;
        var intent = monster.NextMove.Intents
            .OfType<AttackIntent>()
            .FirstOrDefault();
        ArgumentNullException.ThrowIfNull(intent);
        var damage = (decimal)intent.GetSingleDamage([Owner.Creature], monster.Creature);
        var repeat = intent.Repeats;
        await CommonActions.CardAttack(this, cardPlay.Target, damage, repeat).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }
}