using HarmonyLib;
using System.Collections;
using UnityEngine;
using DunGen;
using static LC_InsanityDisplay.UI.MeterHandler;
using static LC_InsanityDisplay.UI.IconHandler;
using LC_InsanityDisplay.ModCompatibility;
using LC_InsanityDisplay.Config;

namespace LC_InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HUDManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostfix()
        {
            CoroutineHelper.Start(CreateInsanityMeter());
        }

        private static IEnumerator CreateInsanityMeter()
        {
            yield return new WaitUntil(() => GameNetworkManager.Instance?.localPlayerController?.sprintMeterUI != null && HUDManager.Instance?.selfRedCanvasGroup != null);
            CreateInScene();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SetMeterValues()
        {
            if (InsanityMeter != null && InsanityMeter.activeSelf != ConfigHandler.ModEnabled.Value)
            {
                InsanityMeter.SetActive(ConfigHandler.ModEnabled.Value);
            }

            if (CompatibilityList.ModInstalled.InfectedCompany && InfectedCompanyCompatibility.modInsanitySlider != null) //if mod is found 
            {
                bool setToActive = !ConfigHandler.ModEnabled.Value || ConfigHandler.alwaysFull.Value || !ConfigHandler.Compat.InfectedCompany.Value || CompatibilityList.ModInstalled.EladsHUD && !ConfigHandler.Compat.EladsHUD.Value && ConfigHandler.Compat.InfectedCompany.Value;
                GameObject infectedCompanySlider = InfectedCompanyCompatibility.modInsanitySlider.gameObject;

                if (infectedCompanySlider.activeSelf != setToActive)
                {
                    infectedCompanySlider.SetActive(setToActive);
                }
            }
            UpdateMeter(imageMeter: InsanityImage, textMeter: EladsHUDCompatibility.InsanityInfo); //elad's will be null if not present
            AdjustIcon();
            EnableCompatibilities(hasCustomBehaviour: true); //only update those that are meant to be updated
        }
    }
}