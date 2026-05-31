using AyaMod.Huoreimu.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace AyaMod.Huoreimu.Enchantment;

public abstract class HuoreimuEnchantment : CustomEnchantmentModel
{
    protected HuoreimuEnchantment(bool showAmount, bool hasExtraCardText)
    {
        ShowAmount = showAmount;
        HasExtraCardText = hasExtraCardText;
        _id = Id.Entry.RemovePrefix().ToLowerInvariant();
    }

    private readonly string _id;

    public override bool ShowAmount { get; }
    public override bool HasExtraCardText { get; }

    protected override string CustomIconPath => $"{_id}.png".EnchantmentIconPath();
}