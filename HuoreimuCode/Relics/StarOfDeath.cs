using AyaMod.Huoreimu.Character;
using AyaMod.Huoreimu.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Relic;

[Pool(typeof(HuoreimuRelicPool))]
public class StarOfDeath : CustomRelicModel
{
    private const decimal OMEN_OF_DEATH_VALUE = 1;
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OmenOfDeathPower>(OMEN_OF_DEATH_VALUE)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OmenOfDeathPower>()];

    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner.Creature)
        {
            return;
        }

        foreach (var result in command.Results)
        {
            Flash();
            await PowerCmd.Apply<OmenOfDeathPower>(result.Receiver, OMEN_OF_DEATH_VALUE, Owner.Creature, null);
        }
    }
}