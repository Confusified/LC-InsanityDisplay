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
        private const string modVersion = "1.0.3";

        private readonly ConfigFile modConfig = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\") + ".cfg"), false);
        public class ModSettings
        {
            public static ConfigEntry<bool> ModEnabled;
            public static ConfigEntry<int> FramerateLimit;
            public static ConfigEntry<int> LastLoggedIndex;
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
                _Harmony.PatchAll(Assembly.GetExecutingAssembly());
                modLogger.LogInfo($"{modName} {modVersion} loaded");
                return;
            }
            else
            {
                modLogger.LogInfo($"{modName} {modVersion} did not load, it is disabled in the config");
                return;
            }
        }

        private void SetDefaultConfigValues()
        {
            ModSettings.ModEnabled = modConfig.Bind<bool>("_Mod Settings", "Enabled", true, "Change the Framerate selector from a dropdown into a slider");
            ModSettings.FramerateLimit = modConfig.Bind<int>("_Mod Settings", "FramerateCap", 60, "The maximum amount of frames that your game will render");
            ModSettings.LastLoggedIndex = modConfig.Bind<int>("Do Not Touch", "LLI", -1, "This is responsible for a seamless transition between modded and vanilla");
        }
    }
}
