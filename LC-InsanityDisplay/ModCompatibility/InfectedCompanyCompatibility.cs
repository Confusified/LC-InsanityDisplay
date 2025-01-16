using LC_InsanityDisplay.Plugin;
using LC_InsanityDisplay.Plugin.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class InfectedCompanyCompatibility : MonoBehaviour
    {
        internal const string ModGUID = "InfectedCompany.InfectedCompany";
        public static GameObject InfectedMeter { get; private set; } = null!;
        public static Slider InfectedMeterComponent { get; private set; } = null!;
        public static bool IsPlayerInfected { get; internal set; } = false;
        public static bool IsInfectedCompanyEnabled { get; private set; } = false;
        public static bool OnlyUseInfectedCompany { get; private set; } = false;

        private static readonly WaitUntil WaitUntilSpawningEnemies = new(() => RoundManager.Instance.currentLevel.spawnEnemiesAndScrap);
        private static readonly WaitForSeconds WaitSetSeconds = new(5f);
        /// <summary>
        /// Fetch the component
        /// then every time UpdateMeter() is called take over after certain conditions
        /// e.g. after always show condition
        /// </summary>
        private static void Initialize()
        {
            //Example
            On.StartOfRound.StartGame += RoundStarted;
            IsInfectedCompanyEnabled = ConfigHandler.Compat.InfectedCompany.Value;
            OnlyUseInfectedCompany = ConfigHandler.Compat.InfectedCompany_InfectedOnly.Value;
            ConfigHandler.Compat.InfectedCompany.SettingChanged += UpdateInfectedCompanyHUD;
            ConfigHandler.Compat.InfectedCompany_InfectedOnly.SettingChanged += ToggleInfectedOnly;
        }
        /// <summary>
        /// Updates the InfectedCompany setting
        /// </summary>
        private static void UpdateInfectedCompanyHUD(object sender, System.EventArgs e)
        {
            IsInfectedCompanyEnabled = ConfigHandler.Compat.InfectedCompany.Value;
            if (!IsInfectedCompanyEnabled)
            {
                if (IsPlayerInfected) InfectedMeter.SetActive(true);
                else if (!OnlyUseInfectedCompany) HUDInjector.InsanityMeter.SetActive(true);
            }
            else
            {
                if (IsPlayerInfected)
                {
                    InfectedMeter.SetActive(false);
                    HUDInjector.InsanityMeter.SetActive(true);
                }
                else if (OnlyUseInfectedCompany) HUDInjector.InsanityMeter.SetActive(false);
            }

            HUDBehaviour.UpdateMeter(settingChanged: true);
        }
        /// <summary>
        /// Updates the InfectedCompany_InfectedOnly setting
        /// </summary>
        private static void ToggleInfectedOnly(object sender, System.EventArgs e)
        {
            OnlyUseInfectedCompany = ConfigHandler.Compat.InfectedCompany_InfectedOnly.Value;
            if (!IsInfectedCompanyEnabled)
            {
                if (IsPlayerInfected) InfectedMeter.SetActive(true);
                if (!OnlyUseInfectedCompany) HUDInjector.InsanityMeter.SetActive(true);
            }
            else if (OnlyUseInfectedCompany && !IsPlayerInfected) HUDInjector.InsanityMeter.SetActive(false);
            else HUDInjector.InsanityMeter.SetActive(true);

            HUDBehaviour.UpdateMeter(settingChanged: true);
        }

        /// <summary>
        /// Method that is called whenever the player loads into a lobby
        /// </summary>
        private static void Start()
        {
            Slider[] sliderList = Object.FindObjectsOfType<Slider>(true);
            foreach (Slider slider in sliderList)
            {
                if (slider.gameObject.name == HUDInjector.ModName)
                {
                    InfectedMeterComponent = slider;
                    InfectedMeter = slider.gameObject;
                    break;
                }
            }
            if (!InfectedMeter) return;
            IsPlayerInfected = false;
            //CoroutineHelper.Start(IsPlayerInfected());
        }

        /// <summary>
        /// Method that is called every time the round starts, with InfectedCompany installed
        /// </summary>
        private static void RoundStarted(On.StartOfRound.orig_StartGame orig, StartOfRound self)
        {
            orig(self);
            if (InfectedMeter) Initialise.Instance.StartCoroutine(CheckIfInfectedOnStart());
        }

        /// <summary>
        /// Method that is called during RoundStarted, that will check if the player is infected
        /// </summary>
        private static IEnumerator CheckIfInfectedOnStart()
        {
            yield return WaitUntilSpawningEnemies;
            yield return WaitSetSeconds;

            if (InfectedMeter.activeSelf) IsPlayerInfected = true;
            else IsPlayerInfected = false;

            if (IsInfectedCompanyEnabled)
            {
                if (IsPlayerInfected) { InfectedMeter.SetActive(false); HUDInjector.InsanityMeter.SetActive(true); }
                else
                {
                    InfectedMeter.SetActive(true);
                    if (OnlyUseInfectedCompany && HUDInjector.InsanityMeter.activeSelf) HUDInjector.InsanityMeter.SetActive(false);
                }
            }
            else if (!OnlyUseInfectedCompany) HUDInjector.InsanityMeter.SetActive(true);
        }
    }
}