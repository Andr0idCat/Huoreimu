using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace AyaMod.Huoreimu.Utils;

public interface IRelicObtainedListener
{
    Task OnRelicObtained(RelicModel relic, Player player);
}
