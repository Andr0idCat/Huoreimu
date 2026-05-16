using System.Collections.Generic;
using System.Threading.Tasks;
using AyaMod.Huoreimu.Actions;
using AyaMod.Huoreimu.Cards;
using AyaMod.Huoreimu.Character;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Relic;

[Pool(typeof(HuoreimuRelicPool))]
public class BloodsBane : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromCard<Huo>()
    ];

    public override async Task AfterObtained()
    {
        await CardsAction.AddCardToDeck<Huo>(Owner, 2);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;

        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
