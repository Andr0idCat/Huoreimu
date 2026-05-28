using AyaMod.Huoreimu.Actions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class MiasmicAbyss() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(14, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.CardBlock(this, DynamicVars.Block, cardPlay);
        await CardsAction.AddCardToPileWhenCombat<Huo>(Owner, PileType.Hand, 1, CombatState);
        await CardsAction.AddCardToPileWhenCombat<Huo>(Owner, PileType.Draw, 1, CombatState, CardPilePosition.Random);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}