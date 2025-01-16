using LC_InsanityDisplay.Plugin.UI;
using System;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// Responsible for the compatibility of An0n Patches
    /// </summary>
    public class An0nPatchesCompatibility
    {
        internal const string ModGUID = "com.an0n.patch";
        internal const string AlternateModGUID = "LethalCompanyPatched";
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
            ConfigHandler.Compat.LethalCompanyPatched.SettingChanged += UpdateAn0nDisplay;
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
            // Make sure that An0n Patches is present and compat is on OR LethalCompanyPatched is present and compat is on
            bool UICompatSetting = CompatibleDependencyAttribute.IsModPresent(ModGUID) && ConfigHandler.Compat.An0nPatches.Value;
            UICompatSetting = UICompatSetting || CompatibleDependencyAttribute.IsModPresent(AlternateModGUID) && ConfigHandler.Compat.LethalCompanyPatched.Value;

            An0nTransform.SetLocalPositionAndRotation(localPosition: UICompatSetting ? localPosition + localPositionOffset : localPosition, localRotation: An0nTransform.localRotation);
        }
    }

}
