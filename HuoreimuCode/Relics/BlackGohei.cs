using System.Collections.Generic;
using System.Threading.Tasks;
using AyaMod.Huoreimu.Character;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Relic;

public class BlackGohei : HuoreimuRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>(1)];

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (target.Side == Owner.Creature.Side) return;

        Flash();
        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, DynamicVars.Power<IntangiblePower>().IntValue, Owner.Creature, null, false);
    }
}
