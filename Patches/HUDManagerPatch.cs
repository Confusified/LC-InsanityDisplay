using HarmonyLib;
using InsanityDisplay.Config;
using InsanityDisplay.ModCompatibility;
using System.Collections;
using UnityEngine;
using DunGen;
using static InsanityDisplay.UI.UIHandler;

namespace InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HUDManagerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix()
        {
            CoroutineHelper.Start(CreateInsanityMeter());
        }

        private static IEnumerator CreateInsanityMeter()
        {
            yield return new WaitUntil(() => GameNetworkManager.Instance.localPlayerController?.sprintMeterUI != null);
            CreateInScene();
        }

        [HarmonyPatch(nameof(HUDManager.OnDestroy))]
        [HarmonyPostfix]
        private static void DestroyInsanityMeter()
        {
            GameObject.Destroy(InsanityMeter?.gameObject);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SetMeterValues()
        {
            if (InsanityMeter != null && InsanityMeter.activeSelf != ConfigSettings.ModEnabled.Value)
            {
                InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);
            }

            if (CompatibilityList.ModInstalled.InfectedCompany && InfectedCompanyCompatibility.modInsanitySlider != null) //if mod is found 
            {
                bool setToActive = !ConfigSettings.ModEnabled.Value || ConfigSettings.alwaysFull.Value || !ConfigSettings.Compat.InfectedCompany.Value;
                GameObject infectedCompanySlider = InfectedCompanyCompatibility.modInsanitySlider.gameObject;

                if (infectedCompanySlider.activeSelf != setToActive)
                {
                    infectedCompanySlider.SetActive(setToActive);
                }
            }
            UpdateMeter(imageMeter: InsanityImage, textMeter: EladsHUDCompatibility.InsanityInfo); //elad's will be null if not present
        }
    }
}