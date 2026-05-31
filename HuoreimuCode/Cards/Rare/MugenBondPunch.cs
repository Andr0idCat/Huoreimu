using AyaMod.Huoreimu.Enchantment;
using AyaMod.Huoreimu.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AyaMod.Huoreimu.Cards.Rare;

public class MugenBondPunch() : HuoreimuCard(0, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(HuoreimuKeywords.UltimateSkill)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(CombatState);
        var rng = Owner.RunState.Rng.Niche;
        foreach (var player in CombatState.Players)
        {
            var cards = PileType.Draw.GetPile(player).Cards
                .Where(c => c.Type is CardType.Attack && c.TargetType is TargetType.AnyEnemy && c.Enchantment is null)
                .ToArray();
            if (cards.Length == 0) continue;
            var result = rng.NextItem(cards);
            CardCmd.Enchant<MugenBondPunchEnchantment>(result, 1);
            CardCmd.Preview(result);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}