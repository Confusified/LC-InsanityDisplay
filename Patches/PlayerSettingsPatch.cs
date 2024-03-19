using HarmonyLib;
using UnityEngine;

namespace FPSSlider.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class PlayerSettingsPatch
    {

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        public static bool RewriteSetFramerateCap(IngamePlayerSettings __instance, int value)
        {
            if (!Initialize.ModSettings.ModEnabled.Value) { return true; } //if the mod is disabled run the vanilla code
            int cap = (int)Initialize.ModSettings.FramerateLimit.Value;

            if (cap == 0)
            {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 1;
                value = 0;
                __instance.settings.framerateCapIndex = value; //set vanilla to VSync to make it more seamless when removing the mod
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                if (cap < 0)
                {
                    Application.targetFrameRate = 1;
                    value = 5;
                    __instance.settings.framerateCapIndex = value; //set vanilla cap to 30 because it's the lowest
                    Initialize.modLogger.LogInfo("Framerate cap was set too small, setting it to 1");
                }
                else if (cap > 500)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    value = 1;
                    __instance.settings.framerateCapIndex = value; //Set vanilla setting to Unlimited
                }
                else
                {
                    Application.targetFrameRate = cap;
                    value = 4;
                    __instance.settings.framerateCapIndex = value; //set to 60 because idk!!!!!!!! (maybe in the future i'll change it to set it to whichever is closest to the selected number (a fix for the future i suppose)
                }
            }
            __instance.unsavedSettings.framerateCapIndex = value;
            return false;
        }
    }
}
