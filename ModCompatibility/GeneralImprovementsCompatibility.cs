using LC_InsanityDisplay.Config;
using UnityEngine;
using static LC_InsanityDisplay.UI.MeterHandler;
using static LC_InsanityDisplay.UI.IconHandler;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class GeneralImprovementsCompatibility : CompatBase
    {
        public static GeneralImprovementsCompatibility Instance { get; private set; } = null!;
        private static GameObject HitpointDisplay;
        private static Vector3 localPosition = Vector3.zero;
        private static Vector3 localPositionOffset = new Vector3(-2f, 28f, 0);

        private static void Initialize()
        {
            Instance = new() { Installed = true };
        }

        public static void MoveHPHud()
        {
            if (!HitpointDisplay)
            {
                HitpointDisplay = TopLeftCornerHUD?.transform.Find("HPUI")?.gameObject;
            }
            if (!HitpointDisplay) { return; }

            localPosition = localPosition == Vector3.zero ? HitpointDisplay.transform.localPosition : localPosition;
            bool GICompat = ConfigHandler.Compat.GeneralImprovements.Value;
            if (GICompat && HitpointDisplay.transform.localPosition != localPosition + localPositionOffset || !GICompat && HitpointDisplay.transform.localPosition != localPosition - selfLocalPositionOffset) //update if hud is positioned incorrectly
            {
                HitpointDisplay.transform.localPosition = GICompat && ConfigHandler.ModEnabled.Value ? localPosition + localPositionOffset : localPosition - selfLocalPositionOffset; //subtract the offset (generalimprovements' positioning is very weird for me without the mod (and with))
            }
        }
    }
}