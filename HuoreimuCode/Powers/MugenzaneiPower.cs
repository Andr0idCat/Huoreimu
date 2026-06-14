using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Powers;

public sealed class MugenzaneiPower() : HuoreimuPowerBase(PowerType.Debuff, PowerStackType.Single)
{
    private CardModel _cardRef;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        _cardRef != null ? [HoverTipFactory.FromCard(_cardRef)] : base.ExtraHoverTips;

    public override Task AfterApplied(Creature applier, CardModel cardSource)
    {
        if (cardSource != null) _cardRef = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (_cardRef == null || Owner?.IsDead != false || CombatManager.Instance.IsOverOrEnding) return;
        if (_cardRef.Owner != player) return;

        Flash();
        await CardCmd.AutoPlay(choiceContext, _cardRef, Owner);
    }
}
