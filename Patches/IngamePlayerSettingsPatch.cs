using HarmonyLib;
using UnityEngine;

namespace FrameCapSlider.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        public static int UnsavedLimit = Initialize.ModSettings.FramerateLimit.Value; //so it doesn't mess up the config whenever you launch the game

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        public static bool RewriteSetFramerateCap(IngamePlayerSettings __instance, int value)
        {
            Initialize.ModSettings.FramerateLimit.Value = UnsavedLimit;
            int cap = (int)Initialize.ModSettings.FramerateLimit.Value;

            if (cap <= 0)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                value = 0;
                __instance.settings.framerateCapIndex = value; //set vanilla to VSync to make it more seamless when removing the mod
                Initialize.modLogger.LogInfo("Framerate cap was set to VSync");
                return false;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                if (cap > 500)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    value = 1;
                    __instance.settings.framerateCapIndex = value; //Set vanilla setting to Unlimited
                    Initialize.modLogger.LogInfo("Framerate cap was set above 500, setting it to -1 (Unlimited)");
                    return false;
                }
                else
                {
                    Application.targetFrameRate = cap;
                    value = 4;
                    __instance.settings.framerateCapIndex = value; //set to 60 because idk!!!!!!!! (maybe in the future i'll change it to set it to whichever is closest to the selected number (a fix for the future i suppose)
                    return false;
                }
            }
        }
    }
}
