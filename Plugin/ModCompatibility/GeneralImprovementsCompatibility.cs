using BepInEx.Bootstrap;
using BepInEx.Configuration;
using LC_InsanityDisplay.Plugin.UI;
using System;
using TMPro;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// Responsible for the compatibility of GeneralImprovements
    /// </summary>
    public class GeneralImprovementsCompatibility
    {
        //GI doesn't have config changes until restarting game, so if health ui is not found never check for it again
        internal const string ModGUID = "ShaosilGaming.GeneralImprovements";

        private static GameObject HitpointDisplay = null!;
        private static Vector3 localPosition = Vector3.zero;
        private static Vector3 localPositionOffset = new(-2f, 28f, 0);

        public static bool HitpointDisplayActive = false;

        private static void Initialize()
        {
            //I could check if it is enabled by using the GeneralImprovements dll but i'm stubborn
            ConfigFile GIConfig = Chainloader.PluginInfos[ModGUID].Instance.Config;
            foreach (ConfigDefinition configDef in GIConfig.Keys)
            {
                if (configDef.Key != "ShowHitPoints") continue;
                GIConfig.TryGetEntry(configDef, out ConfigEntry<bool> ShowHitPointsEntry);
                HitpointDisplayActive = ShowHitPointsEntry.Value;
                break;
            }
            if (!HitpointDisplayActive) return;
            ConfigHandler.Compat.GeneralImprovements.SettingChanged += UpdateDisplayPosition;
        }

        private static void Start()
        {
            if (!HitpointDisplayActive) return; //Don't run if the hitpoints are disabled
            TextMeshProUGUI[] ComponentList = HUDInjector.TopLeftHUD.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI component in ComponentList) //fetch the HitpointDisplay (is there a better for this? probably
            {
                if (component.name == "HP") HitpointDisplay = component.gameObject; break;
            }
            if (!HitpointDisplay) return; //Don't continue if it wasn't able to find the HitpointDisplay
            if (localPosition == Vector3.zero) localPosition = HitpointDisplay.transform.localPosition;

            UpdateDisplayPosition();
        }

        internal static void UpdateDisplayPosition(object sender = null!, EventArgs e = null!)
        {
            if (!HitpointDisplayActive || !HitpointDisplay) return; //can't update it if it's not there
            Transform DisplayTransform = HitpointDisplay.transform;
            DisplayTransform.SetLocalPositionAndRotation(ConfigHandler.Compat.GeneralImprovements.Value ? localPosition + localPositionOffset : localPosition, DisplayTransform.localRotation);
        }
    }
}
