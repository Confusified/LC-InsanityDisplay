using DunGen;
using HarmonyLib;
using LC_InsanityDisplay.ModCompatibility;

namespace LC_InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        /// <summary>
        /// StartOfRoundPatch is only used for the functionality of InfectedCompany's compatibility
        /// </summary>
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
