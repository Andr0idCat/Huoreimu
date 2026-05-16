using AyaMod.Huoreimu.Cards.Uncommon;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace AyaMod.Huoreimu.Powers;

public class ChikushodoPower() : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Chikushodo>();
    protected override bool IsPositive => false;
}
