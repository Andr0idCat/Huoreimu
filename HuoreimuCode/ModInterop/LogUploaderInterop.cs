using BaseLib.Utils.ModInterop;

namespace AyaMod.Huoreimu.ModInterop;

[ModInterop("LogUploader", "LogUploader.Scripts.Util.RegisterHelper")]
public class LogUploaderInterop
{
    public static void RegisterAriyaUploader(string modNamespace, string modId, string modVersion, Uri endPoint,
        string hmacSecretKey)
    {
    }
}