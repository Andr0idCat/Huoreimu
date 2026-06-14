using AyaMod.Huoreimu.Powers;
using AyaMod.Huoreimu.Utils;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class SpellToy() : HuoreimuCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self), IRelicObtainedListener
{
    private const int INITIAL_BLOCK = 4;
    private const int INCREASE_INTERVAL = 2;
    private int _relicObtainedAmount;

    [SavedProperty]
    public int RelicObtainedAmount
    {
        get => _relicObtainedAmount;
        set
        {
            AssertMutable();
            _relicObtainedAmount = value;
            DynamicVars.Power<SpellToyPower>().BaseValue = BlockAmount;
        }
    }

    private int BlockAmount => RelicObtainedAmount / INCREASE_INTERVAL + INITIAL_BLOCK;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SpellToyPower>(BlockAmount),
        new (HuoreimuConstants.INTERVAL, INCREASE_INTERVAL)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.ApplySelf<SpellToyPower>(choiceContext, this, BlockAmount);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    public Task OnRelicObtained(RelicModel relic, Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        var lastBlock = BlockAmount;
        RelicObtainedAmount++;
        if (BlockAmount > lastBlock)
        {
            CardCmd.Preview(this);
        }
        return Task.CompletedTask;
    }
}