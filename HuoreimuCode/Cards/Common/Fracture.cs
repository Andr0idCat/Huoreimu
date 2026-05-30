using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Common;

public class Fracture() : HuoreimuCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int BASE_DAMAGE = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);

        var hand = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this && !c.Keywords.Contains(CardKeyword.Exhaust) &&
                        !c.Keywords.Contains(CardKeyword.Unplayable))
            .ToList();
        var rng = Owner.RunState.Rng.Niche;
        hand.StableShuffle(rng);
        foreach (var card in hand.Take(2))
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
            CardCmd.Preview(card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}