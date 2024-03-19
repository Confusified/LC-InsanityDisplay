using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace FrameCapSlider
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialize : BaseUnityPlugin
    {
        private const string modGUID = "Confusified.FrameCapSlider";
        private const string modName = "Framerate Cap Slider";
        private const string modVersion = "1.0.0";

        private readonly ConfigFile modConfig = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\") + ".cfg"), false);
        public class ModSettings
        {
            public static ConfigEntry<bool> ModEnabled;
            public static ConfigEntry<int> FramerateLimit;
        }

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;
        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            SetDefaultConfigValues();
            if (ModSettings.ModEnabled.Value) { 
                _Harmony.PatchAll(Assembly.GetExecutingAssembly()); 
                modLogger.LogInfo($"{modName} {modVersion} has loaded"); 
            }
            else {
                modLogger.LogInfo($"{modName} {modVersion} did nothing, it is disabled in the config");
            }
        }

        private void SetDefaultConfigValues()
        {
            ModSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Enabled", true, "Change the Framerate selector from a dropdown into a slider");
            ModSettings.FramerateLimit = modConfig.Bind<int>("Mod Settings", "FramerateCap", 60, "The maximum amount of frames that your game will render");
        }
    }
}
