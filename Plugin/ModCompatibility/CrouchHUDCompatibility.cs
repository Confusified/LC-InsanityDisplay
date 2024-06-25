using LC_InsanityDisplay.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{

    /// <summary>
    /// Responsible for the compatibility of CrouchHUD
    /// </summary>
    public class CrouchHUDCompatibility
    {
        internal const string ModGUID = "LCCrouchHUD";

        private static GameObject CrouchHUD = null!;
        private static Transform IconTransform = null!;
        private static Vector3 positionToLocal = Vector3.zero; //Essentially fixes issues that may occur with resolution related stuff
        private static Vector3 localPositionOffset = new(3f, 7f, 0); //Vector3 is a struct so no GC alloc
        private static bool DisableCrouchHUD = false;

        private static void Initialize()
        {
            DisableCrouchHUD = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableCrouchHUD) return;
            ConfigHandler.Compat.LCCrouchHUD.SettingChanged += UpdateIconPosition;
        }

        private static void Start()
        {
            if (DisableCrouchHUD) return;
            Transform PlayerIconTransform = HUDBehaviour.PlayerIcon.transform;
            CrouchHUD = PlayerIconTransform.GetChild(PlayerIconTransform.childCount - 1).gameObject; //CrouchHUD puts itself in the last index, this will make sure it's found
            if (!CrouchHUD) return;
            IconTransform = CrouchHUD.transform;
            if (positionToLocal == Vector3.zero) positionToLocal = IconTransform.localPosition;

            UpdateIconPosition();
        }

        private static void UpdateIconPosition(object sender = null!, EventArgs e = null!)
        {
            if (CrouchHUD == null || IconTransform == null || positionToLocal == Vector3.zero) return; //can't update it if it ain't there

            IconTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.LCCrouchHUD.Value ? positionToLocal + localPositionOffset : positionToLocal, localRotation: IconTransform.localRotation);
        }
    }


}
