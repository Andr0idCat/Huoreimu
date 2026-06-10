using AyaMod.Huoreimu.Utils;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AyaMod.Huoreimu.Cards.Uncommon;

public class ZeroField() : HuoreimuCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDamageReceiveListener
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Huo>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await CommonActions.CardBlock(this, cardPlay);
    }

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (Pile?.Type is PileType.Deck or PileType.None || target != Owner.Creature) return;
        if (props.HasFlag(ValueProp.Unblockable)) return;
        var combatState = target.CombatState;
        if (combatState is null) return;

        var runState = Owner.RunState;
        var finalDamage = Math.Max(amount - target.Block, 0);
        finalDamage = Hook.ModifyHpLostBeforeOsty(runState, combatState, target, finalDamage, props, dealer,
            cardSource, out _);
        finalDamage = Hook.ModifyHpLostAfterOsty(runState, combatState, target, finalDamage, props, dealer, cardSource,
            out _);

        if (finalDamage < target.CurrentHp) return;

        await CardCmd.AutoPlay(choiceContext, this, Owner.Creature);

        await CardCmd.TransformTo<Huo>(this);
        var deckCard = DeckVersion;
        if (deckCard != null) await CardCmd.TransformTo<Huo>(deckCard);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}