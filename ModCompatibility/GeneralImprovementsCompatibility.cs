using InsanityDisplay.Config;
using UnityEngine;
using static InsanityDisplay.UI.MeterHandler;
using static InsanityDisplay.UI.IconHandler;

namespace InsanityDisplay.ModCompatibility
{
    public class GeneralImprovementsCompatibility
    {
        private static GameObject HitpointDisplay;
        private static Vector3 localPosition = Vector3.zero;
        private static Vector3 localPositionOffset = new Vector3(-2f, 28f, 0);

        public static void MoveHPHud()
        {
            if (!HitpointDisplay)
            {
                HitpointDisplay = TopLeftCornerHUD?.transform.Find("HPUI").gameObject;
            }
            if (!HitpointDisplay) { return; }

            localPosition = localPosition == Vector3.zero ? HitpointDisplay.transform.localPosition : localPosition;
            bool GICompat = ConfigSettings.Compat.GeneralImprovements.Value;
            if ((GICompat && HitpointDisplay.transform.localPosition != (localPosition + localPositionOffset)) || (!GICompat && HitpointDisplay.transform.localPosition != (localPosition - selfLocalPositionOffset))) //update if hud is positioned incorrectly
            {
                HitpointDisplay.transform.localPosition = GICompat && ConfigSettings.ModEnabled.Value ? localPosition + localPositionOffset : (localPosition - selfLocalPositionOffset); //subtract the offset 
            }
        }
    }
}