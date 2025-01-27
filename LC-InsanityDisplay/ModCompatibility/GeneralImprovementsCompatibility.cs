using BepInEx.Bootstrap;
using BepInEx.Configuration;
using DunGen;
using LC_InsanityDisplay.Plugin.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
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
        private static Vector3 localPositionOffset = new(0, 22f, 0);

        public static bool HitpointDisplayActive = false;

        private static void Initialize()
        {
            ConfigFile GIConfig = Chainloader.PluginInfos[ModGUID].Instance.Config;
            foreach (ConfigDefinition configDef in GIConfig.Keys)
            {
                if (configDef.Key != "ShowHitPoints") continue;
                GIConfig.TryGetEntry(configDef, out ConfigEntry<bool> ShowHitPointsEntry);
                HitpointDisplayActive = ShowHitPointsEntry.Value;
                break;
            }
            if (!HitpointDisplayActive) return;
            Initialise.Logger.LogDebug("GI's ShowHitPoints is enabled");
            ConfigHandler.Compat.GeneralImprovements.SettingChanged += UpdateDisplayPosition;
            
        }

        private static void Start()
        {
            if (!HitpointDisplayActive) return; //Don't run if the hitpoints are disabled

            HitpointDisplay = HUDInjector.TopLeftHUD.transform.Find("HPUI").gameObject;
            if (!HitpointDisplay)
            {
                Initialise.Logger.LogError("Could not find GI's HP display");
                return; //Don't continue if it wasn't able to find the HitpointDisplay
            }

            if (localPosition == Vector3.zero) localPosition = HitpointDisplay.transform.localPosition;

            UpdateDisplayPosition();

        }

        internal static void UpdateDisplayPosition(object sender = null!, EventArgs e = null!)
        {
            if (!HitpointDisplayActive || !HitpointDisplay) return; //can't update it if it's not there
            Transform DisplayTransform = HitpointDisplay.transform;
            DisplayTransform.SetLocalPositionAndRotation(ConfigHandler.Compat.GeneralImprovements.Value ? localPosition + localPositionOffset : localPosition, DisplayTransform.localRotation);
            Initialise.Logger.LogDebug("Repositioned GI's HP UI");
        }
    }
}
