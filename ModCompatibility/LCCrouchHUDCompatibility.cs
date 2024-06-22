using LC_InsanityDisplay.Config;
using LC_InsanityDisplay.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class LCCrouchHUDCompatibility
    {
        internal const string ModGUID = "LCCrouchHUD";
        private static GameObject CrouchHUD = null!;
        private static Transform IconTransform = null!;
        private static Vector3 positionToLocal = Vector3.zero; //Essentially fixes issues that may occur with resolution related stuff
        private static Vector3 localPositionOffset = (Vector3.right * 3f) + (Vector3.up * 7f); //probably better than doing new Vector3(3f, 7f, 0); (right actually moves it to the left)

        private static void Start()
        {
            Transform PlayerIconTransform = HUDBehaviour.PlayerIcon.transform;
            CrouchHUD = PlayerIconTransform.GetChild(PlayerIconTransform.childCount - 1).gameObject; //CrouchHUD puts itself in the last index, this will make sure it's found
            //CrouchHUD = HUDBehaviour.PlayerIcon.transform.GetComponentsInChildren<Image>(true)[HUDBehaviour.PlayerIcon.transform.childCount].gameObject; //Locates all Image components and selects the one with the correct index, being CrouchHUD
            IconTransform = CrouchHUD.transform;
            if (positionToLocal == Vector3.zero) positionToLocal = IconTransform.localPosition;

            UpdateIconPosition();
        }

        internal static void UpdateIconPosition(object sender = null!, EventArgs e = null!)
        {
            if (CrouchHUD == null || IconTransform == null || positionToLocal == Vector3.zero) return; //can't update it if it ain't there
            IconTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.LCCrouchHUD.Value ? positionToLocal + localPositionOffset : positionToLocal, localRotation: IconTransform.localRotation);
        }
        /*
        public static void MoveCrouchHUD()
        {

            GameObject CrouchHUDIcon = TopLeftCornerHUD.transform.Find("Self/CrouchIcon")?.gameObject;

            if (CrouchHUDIcon == null) { Initialise.Logger.LogError("LCCrouchHud's icon wasn't found"); return; }

            bool CrouchHUDCompat = ConfigHandler.Compat.LCCrouchHUD.Value;
            if (CrouchHUDCompat && CrouchHUDIcon.transform.localPosition != positionToLocal + localPositionOffset || !CrouchHUDCompat && CrouchHUDIcon.transform.localPosition != positionToLocal) //update if hud is positioned incorrectly
            {
                CrouchHUDIcon.transform.localPosition = CrouchHUDCompat && ConfigHandler.ModEnabled.Value ? positionToLocal + localPositionOffset : positionToLocal;
            }
        }
        */
    }
}