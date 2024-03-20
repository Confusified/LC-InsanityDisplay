using HarmonyLib;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    public class QuickMenuManagerPatch
    {
        private static GameObject SettingsPanel; //Where all the settings are found
        public static GameObject Slider; //Slider this mod will be using
        private static GameObject SliderInSlider;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(QuickMenuManager __instance)
        {
            SettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject; //SettingsPanel has been found and put into variable

            GameObject FramerateObject = SettingsPanel.transform.Find("FramerateCap").gameObject;
            FramerateObject.SetActive(false); //Hide the dropdown

            //Creation and positioning of slider
            Slider = SettingsPanel.transform.Find("MasterVolume").gameObject;

            Slider = GameObject.Instantiate(Slider); //Created copy of MasterVolume, this will be as slider for the FPS
            SliderInSlider = Slider.transform.Find("Slider").gameObject;
            Object.DestroyImmediate(SliderInSlider.GetComponent<SettingsOption>()); //Remove SettingsOption component, add custom functionality

            Slider.transform.name = "FramerateSlider";
            Slider.transform.SetParent(SettingsPanel.transform);
            Slider.transform.position = FramerateObject.transform.position + new Vector3(-0.08f, 0.06f, 0f); //Position the slider to slightly above where the dropdown would be
            Slider.transform.localScale = new Vector3(1f, 1f, 1f);
            Slider.transform.Find("Image").localPosition = new Vector3(-53.4f, 0f, 0f); //Offset to a cap of 60 (when cursor is on it)
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = Initialise.setCorrectText(Slider);

            //Add listener to slider, giving it it's functionality
            SliderInSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { Initialise.SliderValueChanged(SettingsPanel, Slider); });

            //Set values of the slider
            SliderInSlider.GetComponent<Slider>().minValue = 0; //0 = VSync
            SliderInSlider.GetComponent<Slider>().maxValue = 501; //250 = unlimited in vanilla, if set to 501 actually set to -1 (unlimited)
            SliderInSlider.GetComponent<Slider>().value = (float)Initialise.ModSettings.FramerateLimit.Value;

            IngamePlayerSettings.Instance.DiscardChangedSettings(); //To avoid issues
        }
    }
}
