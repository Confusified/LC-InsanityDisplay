using GameNetcodeStuff;
using InsanityDisplay.Config;
using UnityEngine;
using static InsanityDisplay.UI.MeterHandler;

namespace InsanityDisplay.UI
{
    public class IconHandler
    {
        public static Vector3 selfLocalPositionOffset = new Vector3(-6.8f, 4f, 0f); // -272.7607 112.2663 -14.2212 = normal    -279.5677f, 116.2748f, -14.2174f
        public static Vector3 selfLocalPosition = Vector3.zero;
        public static void AdjustIcon()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance?.localPlayerController;
            vanillaSprintMeter = localPlayer?.sprintMeterUI?.gameObject;
            if (!vanillaSprintMeter) { return; } //UI is not fully ready yet (i should add a patch to set certain variables to null when exiting a lobby)
            GameObject selfIcon = TopLeftCornerHUD?.transform?.Find("Self")?.gameObject; //Doesn't seem to have a simple variable attached to it
            GameObject selfRedIcon = HUDManager.Instance?.selfRedCanvasGroup?.gameObject;
            if (!selfIcon || !selfRedIcon) { return; }

            selfLocalPosition = selfLocalPosition == Vector3.zero ? selfIcon.transform.localPosition : selfLocalPosition;

            //only check one to reduce the amount of conditions
            //if meter enabled, position wrong, always centered, filled more than the minimum
            if (ConfigSettings.ModEnabled.Value && (selfIcon.transform.localPosition != (selfLocalPosition + selfLocalPositionOffset) && InsanityImage?.fillAmount > accurate_MinValue) || ConfigSettings.iconAlwaysCentered.Value)
            {
                selfRedIcon.transform.localPosition = selfIcon.transform.localPosition = Vector3.Lerp(selfIcon.transform.localPosition, selfLocalPosition + selfLocalPositionOffset, InsanityImage.fillAmount); //move to the offset position
            }
            //if position wrong, not always centered and meter disabled, filled equal or less than the minimum
            else if (((!ConfigSettings.iconAlwaysCentered.Value && selfIcon.transform.localPosition != (selfLocalPosition)) || ((!ConfigSettings.ModEnabled.Value && !ConfigSettings.iconAlwaysCentered.Value))) && InsanityImage?.fillAmount <= accurate_MinValue)
            {
                selfRedIcon.transform.localPosition = selfIcon.transform.localPosition = Vector3.Lerp(selfIcon.transform.localPosition, selfLocalPosition, InsanityImage.fillAmount); //move to the normal position
            }
        }
    }
}
