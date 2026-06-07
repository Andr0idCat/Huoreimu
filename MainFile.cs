using AyaMod.Huoreimu.Config;
using AyaMod.Huoreimu.Utils;
using BaseLib.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace Huoreimu.Huoreimu;

/**
 * Ideas
 *
 * Self Bind
 *
 * Bind effect - square texture based on model size, lines random generated (amount equal to bind amount)
 * shader of transparency of line based on average of point spread of the model
 * colored
 *
 * Bind... rename? Necrobinder kinda overlaps.
 * */
[ModInitializer(nameof(Initialize))]
public class MainFile
{
    public const string ModId = "Huoreimu"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new HuoreimuConfig());
        ExceptionReporter.Initialize("https://sts2-log-receiver.rubikzero0.workers.dev/",
            "55b6feff04ba8d7b94d89dace842089b80cc0e36049188045e04700bd6acf342");
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}