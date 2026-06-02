using MainFile = Huoreimu.Huoreimu.MainFile;
using MegaCrit.Sts2.Core.Logging;

namespace AyaMod.Huoreimu.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string AudioPath(this string path)
    {
        return $"res://{MainFile.ModId}/Audio/{path}";
    }
    
    public static string ImagePath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/{path}";
    }
	public static string HandsImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "Images", "Hands", path);
    }
    public static string CardImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "Images", "Cards", path);
    }

    public static string BigCardImagePath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/Cards/{path}";
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "Images", "Powers", path);
    }
    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "Images", "Powers", "Big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/Relics/{path}";
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(MainFile.ModId, "Images", "Relics", "Big", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/Charui/{path}";
    }

    public static string PowerIconPath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/Powers/{path}";
    }

    public static string EnchantmentIconPath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/Enchantments/{path}";
    }

    public static string PotionPath(this string path)
    {
        return $"res://{MainFile.ModId}/Image/Potions/{path}";
    }
}
