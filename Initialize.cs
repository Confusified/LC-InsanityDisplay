using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace FPSSlider
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialize : BaseUnityPlugin
    {
        private const string modGUID = "Confusified.FPSSlider";
        private const string modName = "FPS Slider";
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

            _Harmony.PatchAll(Assembly.GetExecutingAssembly());
            modLogger.LogInfo($"{modName} {modVersion} has loaded");
        }

        private void SetDefaultConfigValues()
        {
            ModSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Enabled", true, "Change the Framerate selector from a dropdown into a slider");
            ModSettings.FramerateLimit = modConfig.Bind<int>("Mod Settings", "FramerateCap", 60, "The maximum amount of frames that your game will render");
        }
    }
}
