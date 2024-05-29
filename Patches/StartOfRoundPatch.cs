using DunGen;
using HarmonyLib;
using InsanityDisplay.ModCompatibility;

namespace InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {

        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        public static void RoundStartedPostfix(StartOfRound __instance)
        {
            if (CompatibilityList.ModInstalled.InfectedCompany)
            {
                CoroutineHelper.Start(InfectedCompanyCompatibility.ConvertInsanity());
            }
        }

        [HarmonyPatch("EndOfGame")]
        [HarmonyPostfix]
        public static void EndOfRoundPostfix(StartOfRound __instance)
        {
            SetModMeterToNull();
        }

        [HarmonyPatch("OnDestroy")]
        [HarmonyPostfix]
        public static void LeftLobbbyPostfix(StartOfRound __instance)
        {
            SetModMeterToNull();
        }

        private static void SetModMeterToNull()
        {
            if (CompatibilityList.ModInstalled.InfectedCompany && InfectedCompanyCompatibility.modInsanitySlider != null)
            {
                InfectedCompanyCompatibility.modInsanitySlider = null;
            }
        }
    }
}
