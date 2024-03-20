using HarmonyLib;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePrefix(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; }
            SliderHandler.CreateSliderInMemory();
            SliderHandler.CreateSliderInScene();
        }
    }
}
