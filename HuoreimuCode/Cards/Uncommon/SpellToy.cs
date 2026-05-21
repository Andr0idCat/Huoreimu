using AyaMod.Huoreimu.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AyaMod.Huoreimu.Cards;

public class SpellToy() : HuoreimuCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpellToyPower>(choiceContext, this, 1);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        AddKeyword(CardKeyword.Retain);
    }
}