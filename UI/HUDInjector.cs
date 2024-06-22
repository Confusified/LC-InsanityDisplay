using GameNetcodeStuff;
using LC_InsanityDisplay.ModCompatibility;
using UnityEngine;
using UnityEngine.UI;
using static LC_InsanityDisplay.Config.ConfigHandler;
using static LC_InsanityDisplay.UI.HUDBehaviour;
using static LC_InsanityDisplay.UI.HUDInjector;


namespace LC_InsanityDisplay.UI
{
    /// <summary>
    /// This class will be responsible for injecting the insanity meter into the hud
    /// </summary>
    internal class HUDInjector
    {
        internal static PlayerControllerB LocalPlayerInstance { get; private set; } = null!;
        internal static HUDManager HUDManagerInstance { get; private set; } = null!;
        public static GameObject InsanityMeter { get; internal set; } = null!;
        public static Image InsanityMeterComponent { get; internal set; } = null!;
        public static Vector3 localPositionOffset = new(-3.4f, 3.7f, 0f); //-271.076 102.6285 -13.0663 = normal

        private static GameObject VanillaSprintMeter { get; set; } = null!;
        internal static GameObject TopLeftHUD { get; private set; } = null!;
        private const float localScaleMultiplier = 0.86f; //SprintMeter scale is 1.6892 (for all) and my intended scale is 1.4 (so 1.4/1.628 is 0.86f)

        /// <summary>
        /// Inject into the HUD after all necessary variables have been initialised
        /// This method is also called in StartOfRound.OnPlayerConnectedClientRpc(), so only run the code when the insanity meter has not been created yet
        /// Order of execution: https://boxy-svg.com/app/new:PkmnhM-eZH
        /// source: https://discord.com/channels/1168655651455639582/1191246381634043995/1194046052387532810
        /// </summary>
        public static void InjectIntoHud(On.HUDManager.orig_SetSavedValues orig, HUDManager self, int playerObjectId = -1)
        {
            orig(self, playerObjectId);
            if (playerObjectId == -1 || (playerObjectId != -1 && self.localPlayer == self.playersManager.allPlayerScripts[playerObjectId])) //check for the localplayer
            {
                //Set variables for later use
                HUDManagerInstance = self;
                LocalPlayerInstance = StartOfRound.Instance.localPlayerController;
                VanillaSprintMeter = LocalPlayerInstance.sprintMeterUI.gameObject;
                TopLeftHUD = VanillaSprintMeter.transform.parent.gameObject;
                PlayerIcon = TopLeftHUD.transform.Find("Self").gameObject;
                PlayerRedIcon = HUDManagerInstance.selfRedCanvasGroup.gameObject;
                VanillaIconPosition = PlayerIcon.transform.localPosition;
                CurrentPosition = VanillaIconPosition;
                CenteredIconPosition = VanillaIconPosition + IconPositionOffset;


                //Activate all compatibilities (that are present) and have a Start() method
                CompatibleDependencyAttribute.Activate();
                Initialise.Logger.LogDebug("Activated all the compatibilities");

                //Set up the insanity meter
                Initialise.Logger.LogDebug("Setting up insanity meter...");
                CreateMeter();
            }
        }

        private static void CreateMeter() //potentially save the meter for when going back to the menu and into a lobby again?
        {
            if (InsanityMeter) return; //Make sure the meter doesn't already exist

            //Create insanity meter
            InsanityMeter = Object.Instantiate(VanillaSprintMeter, TopLeftHUD.transform);
            InsanityMeter.SetActive(false); //Should prevent it being marked as dirty
            InsanityMeter.name = "InsanityMeter";

            InsanityMeterComponent = InsanityMeter.GetComponentInChildren<Image>();

            //Set the insanity meter in the correct position
            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetAsFirstSibling();
            meterTransform.localPosition = VanillaSprintMeter.transform.localPosition + localPositionOffset;
            meterTransform.localScale *= localScaleMultiplier;

            //Reset values to avoid any issues
            CurrentlySetColor = TransparantColor;
            LastInsanityLevel = -1;
            LastIconPosition = VanillaIconPosition;


            //Update the insanity meter to have the correct visuals
            UpdateMeter(settingChanged: true);
            InsanityMeter.SetActive(true); //Insanity meter is now set up
            Initialise.Logger.LogDebug("Finished setting up the insanity meter");
            //Update the icon to be positioned correctly
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

        internal static Color TransparantColor { get; private set; } = new(0, 0, 0, 0);

        internal static float LastInsanityLevel { get; set; } = -1;
        internal static float InsanityLevel { get; set; }
        internal static float InsanityLevel_AccurateMargin { get; set; }
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
        private static float CurrentFillAmount;
        private static bool NeverCenter;
        private static bool AlwaysCenter;
        private static float CurrentMeterFill;
        //lerp formula: 1.179x^2 - 0.337x + 0.03 (maybe just make it (minValue*CurrentMeterFill)+minValue)
        //x being CurrentMeterFill
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
            CurrentFillAmount = InsanityMeterComponent.fillAmount;
            if (settingChanged ||
                (!SetAlwaysFull && (LastInsanityLevel != LocalPlayerInstance.insanityLevel || (CurrentFillAmount > accurate_MinValue && LastInsanityLevel == 0) || (ReverseEnabled && LastInsanityLevel == 0 && CurrentFillAmount < accurate_MaxValue))) ||
                (SetAlwaysFull && CurrentFillAmount != 1)
               ) //Only update if actually changed (or called by a settingchange)
            {
                CurrentMeterFill = ReturnInsanityLevel();
                CurrentFillAmount = CurrentMeterFill;
                InsanityMeterComponent.fillAmount = CurrentMeterFill;
            }
            if (CurrentlySetColor != InsanityMeterColor) //Only update the colour when it's been changed
            {
                InsanityMeterComponent.color = InsanityMeterColor;
                CurrentlySetColor = InsanityMeterColor;
            }
        }
        /// <summary>
        /// Responsible for returning the correct amount
        /// e.g. when using Accurate Meter
        /// </summary>
        private static float ReturnInsanityLevel()
        {
            if (SetAlwaysFull) return 1;
            InsanityLevel = LocalPlayerInstance.insanityLevel / LocalPlayerInstance.maxInsanityLevel;
            InsanityLevel_AccurateMargin = InsanityLevel * accurate_Diff;
            if (usingAccurateDisplay != useAccurateDisplay.Value) usingAccurateDisplay = useAccurateDisplay.Value;
            if (ReverseEnabled != enableReverse.Value) ReverseEnabled = enableReverse.Value;
            if (usingAccurateDisplay) return !ReverseEnabled ? accurate_MinValue + InsanityLevel_AccurateMargin : accurate_MaxValue - InsanityLevel_AccurateMargin; //Return the accurate version (normal or reversed)

            return !ReverseEnabled ? InsanityLevel : 1 - InsanityLevel; //return normal insanity value (or reversed version)
        }
        /// <summary>
        /// Only updates when the insanityLevel value has changed and the appropriate conditions are met
        /// </summary>
        internal static void InsanityValueChanged(On.GameNetcodeStuff.PlayerControllerB.orig_SetPlayerSanityLevel orig, PlayerControllerB self) //This runs every frame, but will only update when appropriate
        {
            orig(self);
            if (InsanityMeterComponent && !self.isPlayerDead)
            {
                if (SetAlwaysFull != alwaysFull.Value) SetAlwaysFull = alwaysFull.Value;
                if (ReverseEnabled != enableReverse.Value) ReverseEnabled = enableReverse.Value;
                CurrentFillAmount = InsanityMeterComponent.fillAmount;

                if (((CurrentMeterFill != InsanityLevel || LastInsanityLevel != self.insanityLevel || CurrentMeterFill != CurrentFillAmount) && !SetAlwaysFull) ||
                    (SetAlwaysFull && CurrentFillAmount != 1) ||
                    (ReverseEnabled && CurrentFillAmount < accurate_MaxValue && self.insanityLevel == 0))
                    UpdateMeter();

                if (PlayerIcon && PlayerRedIcon) UpdateIconPosition();
                LastInsanityLevel = self.insanityLevel;
            }
        }

        /// <summary>
        /// This is responsible for moving the self icon to the correct position
        /// e.g. being centered when meter is visible or config is set to Always, etc
        /// </summary>
        internal static void UpdateIconPosition(bool settingChanged = false)
        {
            if (IconSetting != iconAlwaysCentered.Value) IconSetting = iconAlwaysCentered.Value;
            if (NeverCenter != (IconSetting == CenteredIconSettings.Never)) NeverCenter = IconSetting == CenteredIconSettings.Never;
            if (AlwaysCenter != (IconSetting == CenteredIconSettings.Always)) AlwaysCenter = IconSetting == CenteredIconSettings.Always;
            CurrentMeterFill = ReturnInsanityLevel();

            // Don't update when not necessary
            if ((LastIconPosition == VanillaIconPosition && NeverCenter) || //If Never Centering and vanilla position
                (LastIconPosition == CenteredIconPosition && AlwaysCenter) || //If Always Centering and centered
                (CurrentMeterFill >= accurate_MaxValue && LastIconPosition == CenteredIconPosition && !NeverCenter) || //If there is no visible change when it's max
                (!usingAccurateDisplay && (CurrentMeterFill < accurate_MinValue && LastIconPosition == VanillaIconPosition) && !AlwaysCenter) || //No visible change without Accurate Display
                (usingAccurateDisplay && (CurrentMeterFill <= accurate_MinValue && LastIconPosition == VanillaIconPosition) && !AlwaysCenter) || //No visible change with Accurate Display
                (CurrentMeterFill < accurate_MaxValue && CurrentMeterFill > accurate_MinValue && LastIconPosition == CenteredIconPosition) //Icon is centered 
                && !settingChanged) return;

            //Determine the position the player icon must be in
            if (NeverCenter || (CurrentMeterFill < accurate_MinValue && NeverCenter)) NewPosition = VanillaIconPosition; //Set it to the vanilla position
            else if (SetAlwaysFull || AlwaysCenter || (CurrentMeterFill > accurate_MaxValue && !NeverCenter)) NewPosition = CenteredIconPosition; //Set it to the centered position
            else //Set it to a certain position in between vanilla and centered
            {
                float lerpValue = (lerpNumber1 * (CurrentMeterFill * CurrentMeterFill)) - (lerpNumber2 * CurrentMeterFill) + lerpNumber3;
                Vector3 targetVector3 = CurrentMeterFill > accurate_MinValue ? CenteredIconPosition : VanillaIconPosition;
                if (Vector3.Distance(NewPosition, targetVector3) <= 0.05f) NewPosition = targetVector3;
                else
                {
                    CurrentPosition = PlayerIcon.transform.localPosition;
                    NewPosition = Vector3.Lerp(CurrentPosition, targetVector3, lerpValue);
                }
            }

            LastIconPosition = NewPosition;
            PlayerIcon.transform.localPosition = PlayerRedIcon.transform.localPosition = NewPosition;
        }
    }
}
