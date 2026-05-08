using AyaMod.Huoreimu.Cards.Common;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Powers;

public class NecroticPower() : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Necrotic>();
    protected override bool IsPositive => false;
}
