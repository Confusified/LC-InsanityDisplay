using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace FPSSlider
{
    [BepInPlugin(modGUID,modName,modVersion)]
    public class Initialize : BaseUnityPlugin
    {
        private const string modGUID = "mod.Confusified.FPSSlider";
        private const string modName = "FPS Slider";
        private const string modVersion = "1.0.0";

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;
        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            _Harmony.PatchAll();
        }
    }
}
