using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        private static GameObject SettingsPanel; //Where all the settings are found
        public static GameObject Slider; //Slider this mod will be using
        private static GameObject SliderInSlider;

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AwakePrefix(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; }

            SettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject; //SettingsPanel has been found and put into variable

            GameObject FramerateObject = SettingsPanel.transform.Find("FramerateCap").gameObject;
            FramerateObject.SetActive(false); //Hide the dropdown

            //Creation and positioning of slider
            Slider = SettingsPanel.transform.Find("MasterVolume").gameObject;

            Slider = GameObject.Instantiate(Slider); //Created copy of MasterVolume, this will be as slider for the FPS
            SliderInSlider = Slider.transform.Find("Slider").gameObject;
            Object.DestroyImmediate(SliderInSlider.GetComponent<SettingsOption>()); //Remove SettingsOption component, add custom functionality

            Slider.transform.name = "FramerateSlider";
            Slider.transform.SetParent(SettingsPanel.transform);
            Slider.transform.position = FramerateObject.transform.position + new Vector3(-1.4f, 1f, 0f); //Position the slider to slightly above where the dropdown would be
            Slider.transform.localScale = new Vector3(1f, 1f, 1f);
            Slider.transform.Find("Image").localPosition = new Vector3(-53.4f,0f,0f); //Offset to a cap of 60 (when cursor is on it)
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = Initialise.setCorrectText(Slider);

            //Add listener to slider, giving it it's functionality
            SliderInSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { Initialise.SliderValueChanged(SettingsPanel, Slider); });

            //Set values of the slider
            SliderInSlider.GetComponent<Slider>().minValue = 0; //0 = VSync
            SliderInSlider.GetComponent<Slider>().maxValue = 501; //501 == Unlimited actually set to -1 (true unlimited), vanilla unlimited would be 250
            SliderInSlider.GetComponent<Slider>().value = (float)Initialise.ModSettings.FramerateLimit.Value;

            IngamePlayerSettings.Instance.DiscardChangedSettings(); //To avoid issues
        }
    }
}
