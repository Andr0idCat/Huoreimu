using AyaMod.Huoreimu.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Debug;

namespace AyaMod.Huoreimu.Patch;

[HarmonyPatch(typeof(SentryService), nameof(SentryService.CaptureException), typeof(Exception))]
public static class ExceptionReportPatch
{
    [HarmonyPrefix]
    private static void OnCaptureException(Exception ex)
    {
        ExceptionReporter.UploadAsync(ex).ConfigureAwait(false);
    }
}
