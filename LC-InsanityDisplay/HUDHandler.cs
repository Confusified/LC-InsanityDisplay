using GameNetcodeStuff;
using LC_InsanityDisplay.ModCompatibility;
using LC_InsanityDisplay.Plugin.ModCompatibility;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UI;
using static LC_InsanityDisplay.Plugin.ConfigHandler;
using static LC_InsanityDisplay.Plugin.UI.HUDBehaviour;
using static LC_InsanityDisplay.Plugin.UI.HUDInjector;


namespace LC_InsanityDisplay.Plugin.UI
{
    /// <summary>
    /// This class will be responsible for injecting the insanity meter into the hud
    /// </summary>
    public class HUDInjector
    {
        internal static PlayerControllerB LocalPlayerInstance { get; private set; } = null!;
        internal static HUDManager HUDManagerInstance { get; private set; } = null!;
        internal static GameObject TopLeftHUD { get; private set; } = null!;
        public const string ModName = "InsanityMeter";

        public static GameObject InsanityMeter { get; internal set; } = null!;
        public static Image InsanityMeterComponent { get; internal set; } = null!;
        public static Vector3 localPositionOffset = new(-3.4f, 3.7f, 0f); //-271.076 102.6285 -13.0663 = normal

        private static GameObject VanillaSprintMeter { get; set; } = null!;
        private const float localScaleMultiplier = 0.86f; //SprintMeter scale is 1.6892 (for all) and my intended scale is 1.4 (so 1.4/1.628 is 0.86f)

        /// <summary>
        /// Inject into the HUD after all necessary variables have been initialised
        /// This method is also called in StartOfRound.OnPlayerConnectedClientRpc(), so only run the code when the connected player is the localplayer
        /// Order of execution: https://boxy-svg.com/app/ > Import (import the .svg file from the source)
        /// source: https://discord.com/channels/1168655651455639582/1191246381634043995/1194046052387532810
        /// </summary>
        public static void InjectIntoHud(On.HUDManager.orig_SetSavedValues orig, HUDManager self, int playerObjectId = -1)
        {
            orig(self, playerObjectId);
            if (playerObjectId == -1 || playerObjectId != -1 && self.localPlayer == self.playersManager.allPlayerScripts[playerObjectId]) //only continue if the player who connected is the localplayer
            {
                // Set variables for later use
                HUDManagerInstance = self;
                LocalPlayerInstance = StartOfRound.Instance.localPlayerController;
                VanillaSprintMeter = LocalPlayerInstance.sprintMeterUI.gameObject;
                TopLeftHUD = VanillaSprintMeter.transform.parent.gameObject;
                PlayerIcon = TopLeftHUD.transform.Find("Self").gameObject;
                PlayerRedIcon = HUDManagerInstance.selfRedCanvasGroup.gameObject;
                VanillaIconPosition = PlayerIcon.transform.localPosition;
                CurrentPosition = VanillaIconPosition;
                CenteredIconPosition = VanillaIconPosition + IconPositionOffset;

                // Activate all compatibilities (that are present) and have a Start() method
                Initialise.Logger.LogDebug("Activating compatibilities...");
                CompatibleDependencyAttribute.Activate();
                Initialise.Logger.LogDebug("Activated all the compatibilities");

                // Set up the insanity meter
                Initialise.Logger.LogDebug("Setting up insanity meter...");
                CreateMeter();
                Initialise.Logger.LogDebug("Finished setting up the insanity meter");
            }
        }

        private static void CreateMeter()
        {
            if (!InsanityMeter) // If the Insanity meter doesn't already exist, create it.
            {

                // Create insanity meter
                InsanityMeter = Object.Instantiate(VanillaSprintMeter, TopLeftHUD.transform);
                InsanityMeter.SetActive(false);
                InsanityMeter.name = ModName;

                InsanityMeterComponent = InsanityMeter.GetComponentInChildren<Image>(true);

                // Set the insanity meter in the correct position
                Transform meterTransform = InsanityMeter.transform;
                meterTransform.SetAsFirstSibling();
                meterTransform.SetLocalPositionAndRotation(VanillaSprintMeter.transform.localPosition + localPositionOffset, meterTransform.localRotation);
                meterTransform.localScale *= localScaleMultiplier;


            }

            // Reset values to avoid any issues
            CurrentlySetColor = new(0, 0, 0, 0); //The way the mod is currently set up doesn't allow for a transparant meter
            LastInsanityLevel = -1;
            LastIconPosition = VanillaIconPosition;

            // Update the insanity meter to have the correct visuals
            UpdateMeter(settingChanged: true);
            if (!InfectedCompanyCompatibility.InfectedMeter || !InfectedCompanyCompatibility.IsInfectedCompanyEnabled || !InfectedCompanyCompatibility.OnlyUseInfectedCompany)
            {
                InsanityMeter.SetActive(true);
            }

            // Update the icon to be positioned correctly
            UpdateIconPosition(settingChanged: true);

        }
    }

    /// <summary>
    /// This class will be responsible for all the behaviour of the insanity meter and the player icon.
    /// Compatibilities not included, probably
    /// </summary>
    internal class HUDBehaviour
    {
        public const float accurate_MinValue = 0.2976f; //Becomes visible starting 0.2976f
        public const float accurate_MaxValue = 0.9101f; //No visible changes after this value
        public const float accurate_Diff = accurate_MaxValue - accurate_MinValue;
        public static Color InsanityMeterColor { get; internal set; }
        public static Color CurrentlySetColor { get; internal set; }
        public static GameObject PlayerIcon { get; internal set; } = null!;
        public static GameObject PlayerRedIcon { get; internal set; } = null!;

        internal static float LastInsanityLevel { get; set; } = -1;
        internal static float InsanityLevel { get; set; }
        internal static Vector3 LastIconPosition { get; set; }
        internal static Vector3 IconPositionOffset { get; private set; } = new Vector3(-6.8f, 4f, 0f);
        internal static Vector3 VanillaIconPosition { get; set; }
        internal static Vector3 CenteredIconPosition { get; set; }

        internal static bool SetAlwaysFull;
        internal static bool ReverseEnabled;
        internal static bool usingAccurateDisplay;
        internal static Vector3 CurrentPosition;

        private static CenteredIconSettings IconSetting;
        private static Vector3 NewPosition;
        private static bool NeverCenter;

        private static bool AlwaysCenter;
        internal static float CurrentMeterFill;
        // lerp formula: 1.179x^2 - 0.337x + 0.03 (could be made simpler but i'm a sucker for the animation)
        // x being CurrentMeterFill
        private const float lerpNumber1 = 1.179f;
        private const float lerpNumber2 = 0.337f;
        private const float lerpNumber3 = 0.03f;
        /// <summary>
        /// Updates the visuals of the insanity meter
        /// While not setting values when unnecessary
        /// </summary>
        internal static void UpdateMeter(bool settingChanged = false)
        {
            if (!LocalPlayerInstance || !InsanityMeterComponent) return;
            if (settingChanged ||
                !SetAlwaysFull && (LastInsanityLevel != LocalPlayerInstance.insanityLevel || CurrentMeterFill > accurate_MinValue && LastInsanityLevel == 0 || ReverseEnabled && LastInsanityLevel == 0 && CurrentMeterFill < accurate_MaxValue) ||
                SetAlwaysFull && CurrentMeterFill != 1) //Only update if actually changed (or called by a settingchange)
            {
                if ((!SetAlwaysFull && InsanityMeter.activeSelf) || SetAlwaysFull)
                {
                    CurrentMeterFill = ReturnInsanityLevel();
                    InsanityMeterComponent.fillAmount = CurrentMeterFill;
                    if (EladsHUDCompatibility.InsanityPercentageObject) EladsHUDCompatibility.UpdatePercentageText();
                }
            }
            if (CurrentlySetColor != InsanityMeterColor) //Only update the colour when it's been changed
            {
                InsanityMeterComponent.color = InsanityMeterColor;
                CurrentlySetColor = InsanityMeterColor;
                if (EladsHUDCompatibility.InsanityPercentageObject) EladsHUDCompatibility.UpdateColour();
            }
        }
        /// <summary>
        /// Responsible for returning the correct amount
        /// e.g. when using Accurate Meter
        /// </summary>
        private static float ReturnInsanityLevel()
        {
            if (SetAlwaysFull) return 1;
            // Determine if to use vanilla's insanity or InfectedCompany's insanity
            if (InfectedCompanyCompatibility.InfectedMeter && InfectedCompanyCompatibility.IsInfectedCompanyEnabled && InfectedCompanyCompatibility.IsPlayerInfected)
            {
                InsanityLevel = InfectedCompanyCompatibility.InfectedMeterComponent.value;
            }
            else InsanityLevel = LocalPlayerInstance.insanityLevel / LocalPlayerInstance.maxInsanityLevel;

            if (usingAccurateDisplay != useAccurateDisplay.Value) usingAccurateDisplay = useAccurateDisplay.Value;
            if (ReverseEnabled != enableReverse.Value) ReverseEnabled = enableReverse.Value;
            if (usingAccurateDisplay && !EladsHUDCompatibility.InsanityPercentageObject)// Return the accurate version (normal or reversed)
            {
                float InsanityLevel_AccurateMargin = InsanityLevel * accurate_Diff;
                return !ReverseEnabled ? accurate_MinValue + InsanityLevel_AccurateMargin : accurate_MaxValue - InsanityLevel_AccurateMargin;
            }

            return !ReverseEnabled ? InsanityLevel : 1 - InsanityLevel; // Return normal insanity value (or reversed version)
        }
        /// <summary>
        /// Only updates when the insanityLevel value has changed and the appropriate conditions are met
        /// </summary>
        internal static void InsanityValueChanged(On.GameNetcodeStuff.PlayerControllerB.orig_SetPlayerSanityLevel orig, PlayerControllerB self) // This runs every frame, but will only update when appropriate
        {
            orig(self);
            if (!InsanityMeterComponent || self.isPlayerDead || !self.isPlayerControlled) return;

            if (SetAlwaysFull != alwaysFull.Value) SetAlwaysFull = alwaysFull.Value;
            if (ReverseEnabled != enableReverse.Value) ReverseEnabled = enableReverse.Value;

            float CurrentFillAmount = InsanityMeterComponent.fillAmount;
            float currentInsanityLevel = self.insanityLevel;

            if ((CurrentMeterFill != InsanityLevel || LastInsanityLevel != currentInsanityLevel || CurrentMeterFill != CurrentFillAmount) && !SetAlwaysFull ||
                SetAlwaysFull && CurrentFillAmount != 1 ||
                ReverseEnabled && CurrentFillAmount < accurate_MaxValue && currentInsanityLevel == 0 && InsanityMeter.activeSelf)
                UpdateMeter();

            if (PlayerIcon && PlayerRedIcon) UpdateIconPosition();

            if (LastInsanityLevel != currentInsanityLevel) LastInsanityLevel = currentInsanityLevel;
        }

        // Odd bug, icon doesn't start with the correct position only when SetAlwaysFull is enabled
        // e.g. SetAlwaysFull = true, wrong start position
        // SanityMeter = true, correct start position 

        /// <summary>
        /// This is responsible for moving the self icon to the correct position
        /// e.g. being centered when meter is visible or config is set to Always, etc
        /// </summary>
        internal static void UpdateIconPosition(bool settingChanged = false)
        {
            if (CompatibleDependencyAttribute.IsEladsHudPresent) return; // The Player Icon is not visible when using Elad's HUD, so not need to update at all
            if (IconSetting != iconAlwaysCentered.Value) IconSetting = iconAlwaysCentered.Value;
            if (NeverCenter != (IconSetting == CenteredIconSettings.Never)) NeverCenter = IconSetting == CenteredIconSettings.Never;
            if (AlwaysCenter != (IconSetting == CenteredIconSettings.Always)) AlwaysCenter = IconSetting == CenteredIconSettings.Always;
            if (settingChanged) CurrentMeterFill = ReturnInsanityLevel();

            // Don't update when not necessary
            if ((LastIconPosition == VanillaIconPosition && NeverCenter || // If Never Centering and vanilla position
                LastIconPosition == CenteredIconPosition && AlwaysCenter || // If Always Centering and centered
                CurrentMeterFill >= accurate_MaxValue && LastIconPosition == CenteredIconPosition && !NeverCenter || // If there is no visible change when it's max
                !usingAccurateDisplay && CurrentMeterFill < accurate_MinValue && LastIconPosition == VanillaIconPosition && !AlwaysCenter || // No visible change without Accurate Display
                usingAccurateDisplay && CurrentMeterFill <= accurate_MinValue && LastIconPosition == VanillaIconPosition && !AlwaysCenter || // No visible change with Accurate Display
                CurrentMeterFill < accurate_MaxValue && CurrentMeterFill > accurate_MinValue && LastIconPosition == CenteredIconPosition || // Icon is centered 
                InfectedCompanyCompatibility.IsInfectedCompanyEnabled && InfectedCompanyCompatibility.InfectedMeter && !InsanityMeter.activeSelf && LastIconPosition != VanillaIconPosition) // Don't update if InfectedCompany active, meter is disabled 
                && !settingChanged) return;

            // Determine the position the player icon must be in
            if (NeverCenter || CurrentMeterFill < accurate_MinValue && !AlwaysCenter) NewPosition = VanillaIconPosition; //Set it to the vanilla position
            else if (SetAlwaysFull || AlwaysCenter || CurrentMeterFill > accurate_MaxValue && !NeverCenter) NewPosition = CenteredIconPosition; //Set it to the centered position
            else // Set it to a certain position in between vanilla and centered
            {
                float lerpValue = lerpNumber1 * (CurrentMeterFill * CurrentMeterFill) - lerpNumber2 * CurrentMeterFill + lerpNumber3;
                Vector3 targetVector3 = CurrentMeterFill > accurate_MinValue ? CenteredIconPosition : VanillaIconPosition;
                if (!InsanityMeter.activeSelf && targetVector3 != VanillaIconPosition) targetVector3 = VanillaIconPosition; // Set the VanillaIconPosition if the meter is not visible and isn't already set as it
                if (Vector3.Distance(NewPosition, targetVector3) <= 0.05f) NewPosition = targetVector3;
                else
                {
                    CurrentPosition = PlayerIcon.transform.localPosition;
                    NewPosition = Vector3.Lerp(CurrentPosition, targetVector3, lerpValue);
                }
            }

            PlayerIcon.transform.localPosition = PlayerRedIcon.transform.localPosition = NewPosition;
            LastIconPosition = NewPosition;
        }
    }
}
