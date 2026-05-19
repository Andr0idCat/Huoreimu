using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace AyaMod.Huoreimu.Utils;

public static class CustomHoverTip
{
    public static IHoverTip Artist(string artist)
    {
        var locString = new LocString("static_hover_tips", "AYAMOD-ARTIST");
        locString.Add("ArtistName", artist);
        return new HoverTip(locString);
    }
}