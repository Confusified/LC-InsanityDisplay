using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using FramerateSlider.Patches;

namespace FramerateSlider
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "Confusified.FramerateSlider";
        private const string modName = "Framerate Slider";
        private const string modVersion = "1.0.2";

        private readonly ConfigFile modConfig = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\") + ".cfg"), false);
        public class ModSettings
        {
            public static ConfigEntry<bool> ModEnabled;
            public static ConfigEntry<int> FramerateLimit;
            public static ConfigEntry<bool> FirstTime;
        }

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;
        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            SetDefaultConfigValues();

            if (ModSettings.ModEnabled.Value)
            {

                if (ModSettings.FirstTime.Value)
                {
                    IngamePlayerSettingsPatch.EnableFirstTime();
                    ModSettings.FirstTime.Value = false;
                }

                _Harmony.PatchAll(Assembly.GetExecutingAssembly());
                modLogger.LogInfo($"{modName} {modVersion} loaded");
            }
            else
            {
                modLogger.LogInfo($"{modName} {modVersion} did not load, it is disabled in the config");
            }
        }

        private void SetDefaultConfigValues()
        {
            ModSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Enabled", true, "Change the Framerate selector from a dropdown into a slider");
            ModSettings.FramerateLimit = modConfig.Bind<int>("Mod Settings", "FramerateCap", 60, "The maximum amount of frames that your game will render");
            ModSettings.FirstTime = modConfig.Bind<bool>("Do Not Touch", "FirstTime", true, "Please do not touch this");
        }
    }
}
