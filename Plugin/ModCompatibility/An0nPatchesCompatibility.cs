using LC_InsanityDisplay.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// Responsible for the compatibility of An0n Patches
    /// </summary>
    public class An0nPatchesCompatibility
    {
        internal const string ModGUID = "com.an0n.patch";
        private static GameObject An0nTextHUD = null!;
        private static Transform An0nTransform = null!;
        private static Vector3 localPositionOffset = new(3f, 15f, 0);
        private static Vector3 localPosition = Vector3.zero;
        private static bool DisableAn0nHud = false;

        private static void Initialize()
        {
            DisableAn0nHud = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableAn0nHud) return; //Don't run if Elad's HUD is present or ... or ...
            ConfigHandler.Compat.An0nPatches.SettingChanged += UpdateAn0nDisplay;
        }

        private static void Start()
        {
            if (DisableAn0nHud) return;
            Animator[] ComponentList = HUDInjector.TopLeftHUD.GetComponentsInChildren<Animator>(true);
            foreach (Animator component in ComponentList) //fetch the HitpointDisplay (is there a better for this? probably
            {
                if (component.name != "HPSP") continue; //An0nPatches' HUD has three of these in the init and four in the update
                An0nTextHUD = component.gameObject;
                break;
            }
            if (!An0nTextHUD) return;
            An0nTransform = An0nTextHUD.transform;
            if (localPosition == Vector3.zero) localPosition = An0nTransform.localPosition;

            UpdateAn0nDisplay();
        }

        private static void UpdateAn0nDisplay(object sender = null!, EventArgs e = null!)
        {
            if (An0nTextHUD == null || An0nTransform == null || localPosition == Vector3.zero) return; //can't update it if it ain't there

            An0nTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.An0nPatches.Value ? localPosition + localPositionOffset : localPosition, localRotation: An0nTransform.localRotation);
        }
    }

}
