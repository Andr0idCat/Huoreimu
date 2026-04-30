using AyaMod.Huoreimu.Character;
using AyaMod.Huoreimu.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace AyaMod.Huoreimu.Relic;

[Pool(typeof(HuoreimuRelicPool))]
public class StarOfDeath : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("OmenOfDeath", 1)];

    public override async Task AfterAttack(AttackCommand command)
    {
        foreach (var result in command.Results)
        {
            await PowerCmd.Apply<OmenOfDeathPower>(result.Receiver, 1, Owner.Creature, null);
        }
    }
}