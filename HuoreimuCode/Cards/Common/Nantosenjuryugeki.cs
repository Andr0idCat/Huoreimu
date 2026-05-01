using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class Nantosenjuryugeki() : HuoreimuCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new BlockVar(4, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target?.Monster);
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        if (cardPlay.Target.Monster.IntendsToAttack)
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
        else
        {
            await CommonActions.Draw(this, choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}