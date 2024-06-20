using LC_InsanityDisplay;
using LC_InsanityDisplay.Config;
using UnityEngine;
using static LC_InsanityDisplay.UI.MeterHandler;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class An0nPatchesCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(3f, 15f, 0);
        private static Vector3 localPosition = Vector3.zero;

        public static void MoveTextHUD()
        {
            if (CompatibilityList.ModInstalled.EladsHUD) { return; }

            GameObject An0nTextHUD = TopLeftCornerHUD?.transform.Find("HPSP")?.gameObject;

            if (An0nTextHUD == null)
            {
                Initialise.Logger.LogError("An0nTextHUD's HUD wasn't found"); return;
            }

            bool An0nCompat = ConfigHandler.Compat.An0nPatches.Value;
            localPosition = localPosition == Vector3.zero ? An0nTextHUD.transform.localPosition : localPosition;
            if (An0nCompat && An0nTextHUD.transform.localPosition != localPosition + localPositionOffset || !An0nCompat && An0nTextHUD.transform.localPosition != localPosition) //update if hud is positioned incorrectly
            {
                An0nTextHUD.transform.localPosition = An0nCompat && ConfigHandler.ModEnabled.Value ? localPosition + localPositionOffset : localPosition;
            }
        }
    }
}