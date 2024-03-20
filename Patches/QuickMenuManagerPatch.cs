using HarmonyLib;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    public class QuickMenuManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart()
        {
            SliderHandler.CreateSliderInMemory(); //if for some reason it ain't there
            SliderHandler.CreateSliderInScene();
        }
    }
}
