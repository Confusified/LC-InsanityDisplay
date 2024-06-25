using GameNetcodeStuff;
using LC_InsanityDisplay.Plugin.UI;
using System;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    public class ShyHUDCompatibility
    {
        internal const string ModGUID = "ShyHUD";
        internal static float CurrentTransparency = 1f; // Starts as opaque
        internal static bool ShyHUDEnabled;

        private static CanvasRenderer InsanityMeterCanvasRenderer { get; set; } = null!;
        private static bool FadeToZero = false;
        private static bool FadeToOne = false;

        private static void Initialize()
        {
            CompatibleDependencyAttribute.ShyHUDPresent = true;
            if (CompatibleDependencyAttribute.IsEladsHudPresent) return;
            UpdateSetting();
            On.GameNetcodeStuff.PlayerControllerB.LateUpdate += UpdateMeterFade;
            ConfigHandler.Compat.ShyHUD.SettingChanged += UpdateSetting;
        }

        private static void Start()
        {
            CurrentTransparency = 1f;
            // More stuff?
        }
        // Useful info (def helped me understanding why certain conditions were not working)
        // https://forum.unity.com/threads/crossfadealpha-help.924332/
        internal static void UpdateMeterFade(On.GameNetcodeStuff.PlayerControllerB.orig_LateUpdate orig, PlayerControllerB self)
        {
            orig(self);
            if (!HUDInjector.InsanityMeter) return;
            if (!InsanityMeterCanvasRenderer) InsanityMeterCanvasRenderer = HUDInjector.InsanityMeter.GetComponent<CanvasRenderer>();
            if (!InsanityMeterCanvasRenderer || !self.IsOwner || (self.IsServer && !self.isHostPlayerObject) || !self.isPlayerControlled || self.isPlayerDead) return; // Matches the conditions of ShyHUD, to make sure it's synced up (plus my own conditions)

            float CurrentFillAmount = HUDInjector.InsanityMeterComponent.fillAmount;
            float CurrentAlpha = InsanityMeterCanvasRenderer.GetAlpha();
            if (ShyHUDEnabled && CurrentTransparency > 0 && (!HUDBehaviour.SetAlwaysFull && (HUDBehaviour.CurrentMeterFill >= HUDBehaviour.accurate_MaxValue || HUDBehaviour.CurrentMeterFill <= HUDBehaviour.accurate_MinValue)) && !FadeToZero) // Meter is the same as that amount and not transparent yet
            {
                FadeToZero = true;
                FadeToOne = false;
                HUDInjector.InsanityMeterComponent.CrossFadeAlpha(0f, 5f, false);
                CurrentTransparency = CurrentAlpha;
            }
            else if ((HUDBehaviour.CurrentMeterFill > HUDBehaviour.accurate_MinValue && HUDBehaviour.CurrentMeterFill < HUDBehaviour.accurate_MaxValue && !FadeToOne) || (HUDBehaviour.SetAlwaysFull && !FadeToOne)) //!= 1
            {
                FadeToZero = false;
                FadeToOne = true;
                HUDInjector.InsanityMeterComponent.CrossFadeAlpha(1f, 0.5f, false);
                CurrentTransparency = CurrentAlpha;
            }

            if (CurrentAlpha >= .9999f) // Slightly speed up the last few parts as they changes are basically impossible to see anyway
            {
                FadeToOne = false;
                InsanityMeterCanvasRenderer.SetAlpha(1);
                CurrentTransparency = 1;
            }
            else if (CurrentAlpha < .0001f)
            {
                FadeToZero = false;
                InsanityMeterCanvasRenderer.SetAlpha(0);
                CurrentTransparency = 0;
            }
        }

        private static void UpdateSetting(object sender = null!, EventArgs e = null!)
        {
            ShyHUDEnabled = ConfigHandler.Compat.ShyHUD.Value;
        }
    }
}
