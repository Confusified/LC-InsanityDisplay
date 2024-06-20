using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.UI;
using static LC_InsanityDisplay.UI.HUDInjector;
using static LC_InsanityDisplay.UI.HUDBehaviour;
using static LC_InsanityDisplay.Config.ConfigHandler;


namespace LC_InsanityDisplay.UI
{
    /// <summary>
    /// This class will be responsible for injecting the insanity meter into the hud
    /// </summary>
    internal class HUDInjector
    {
        public static PlayerControllerB LocalPlayerInstance { get; internal set; } = null!;
        public static HUDManager HUDManagerInstance { get; internal set; } = null!;
        public static GameObject InsanityMeter { get; internal set; } = null!;
        public static Image InsanityMeterComponent { get; internal set; } = null!;
        public static Vector3 localPositionOffset = new(-3.4f, 3.7f, 0f); //-271.076 102.6285 -13.0663 = normal

        private static GameObject VanillaSprintMeter { get; set; } = null!;
        private static GameObject TopLeftHUD { get; set; } = null!;
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
            if (!InsanityMeter)
            {
                HUDManagerInstance = self;
                LocalPlayerInstance = GameNetworkManager.Instance.localPlayerController;
                Initialise.Logger.LogDebug("Setting up insanity meter...");
                CreateMeter();
            }
        }

        private static void CreateMeter() //potentially save the meter for when going back to the menu and into a lobby again?
        {
            if (InsanityMeter != null) { return; } //Already exists, alternate: if exists change the hide flag and put it in the correct location etc

            VanillaSprintMeter = LocalPlayerInstance.sprintMeterUI.gameObject;
            TopLeftHUD = VanillaSprintMeter.transform.parent.gameObject;

            InsanityMeter = Object.Instantiate(VanillaSprintMeter);
            InsanityMeter.name = "InsanityMeter";

            InsanityMeterComponent = InsanityMeter.GetComponentInChildren<Image>();

            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetParent(parent: TopLeftHUD.transform, worldPositionStays: false);
            meterTransform.SetAsFirstSibling();
            meterTransform.localPosition = VanillaSprintMeter.transform.localPosition + localPositionOffset;
            meterTransform.localScale *= localScaleMultiplier;
            //Reset values to avoid any issues
            CurrentlySetColor = TransparantColor; //This colour is not possible with the way I have everything set up
            LastInsanityLevel = -1; //This value is not possible, normally at least

            //Update the insanity meter to have the correct visuals
            UpdateMeter();
            Initialise.Logger.LogDebug("Finished setting up the insanity meter");
        }
    }

    /// <summary>
    /// This class will be responsible for all the behaviour of the insanity meter and the player icon
    /// Compatibilities not included, probably
    /// </summary>
    internal class HUDBehaviour
    {
        public const float accurate_MinValue = 0.2976f; //Becomes visible starting 0.2976f
        public const float accurate_MaxValue = 0.9101f; //No visible changes after this value
        public const float accurate_Diff = accurate_MaxValue - accurate_MinValue;
        public static Color InsanityMeterColor { get; internal set; }
        public static Color CurrentlySetColor { get; internal set; }

        internal static Color TransparantColor { get; set; } = new(0, 0, 0, 0);

        internal static float LastInsanityLevel { get; set; } = -1;
        /// <summary>
        /// Updates the visuals of the insanity meter
        /// While not setting values when unnecessary
        /// </summary>
        internal static void UpdateMeter(bool settingChanged = false)
        {
            if (LastInsanityLevel != LocalPlayerInstance.insanityLevel || settingChanged) InsanityMeterComponent.fillAmount = ReturnInsanityLevel(); //Only update if actually changed (this is because of the settingchanged event in the confighandler)
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
            if (alwaysFull.Value) return 1; //Return the meter being full
            bool ReverseEnabled = enableReverse.Value;
            float InsanityLevel = LocalPlayerInstance.insanityLevel / LocalPlayerInstance.maxInsanityLevel;
            float InsanityLevel_AccurateMargin = InsanityLevel * accurate_Diff;
            if (useAccurateDisplay.Value) return !ReverseEnabled ? accurate_MinValue + InsanityLevel_AccurateMargin : accurate_MaxValue - InsanityLevel_AccurateMargin; //Return the accurate version (normal or reversed)

            return !ReverseEnabled ? InsanityLevel : 1 - InsanityLevel; //return normal insanity value (or reversed version)
        }
        /// <summary>
        /// Only updates when the insanityLevel value has changed and alwaysFull isn't enabled
        /// </summary>
        internal static void InsanityValueChanged(On.GameNetcodeStuff.PlayerControllerB.orig_SetPlayerSanityLevel orig, PlayerControllerB self) //This runs every frame :( because it's called by PlayerControllerB.Update
        {
            LastInsanityLevel = self.insanityLevel;
            orig(self);
            if (LastInsanityLevel != self.insanityLevel && !alwaysFull.Value) UpdateMeter();
        }
    }
}
