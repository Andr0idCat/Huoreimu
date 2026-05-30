using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class ViolateShot() : HuoreimuCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private int CurseDrawnCount { get; set; }
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(10),
        new ExtraDamageVar(3),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((_, _) => CurseDrawnCount)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay.Target, DynamicVars.CalculatedDamage).Execute(choiceContext);
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Type is CardType.Curse) CurseDrawnCount++;
        return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
}