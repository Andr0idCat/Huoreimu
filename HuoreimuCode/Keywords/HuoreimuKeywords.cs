using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace AyaMod.Huoreimu.Keywords;

public class HuoreimuKeywords
{
    [CustomEnum("ULTIMATE_SKILL")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword UltimateSkill;

    [CustomEnum("FREE_ULTIMATE_SKILL")]
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword FreeUltimateSkill;
}