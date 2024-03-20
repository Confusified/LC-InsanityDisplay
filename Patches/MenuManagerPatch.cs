using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        static GameObject SettingsPanel; //Where all the settings are found
        public static GameObject Slider; //Slider this mod will be using
        
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void OnAwake(MenuManager __instance)
        {
            if (__instance.isInitScene) { return; }

            SettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject; //SettingsPanel has been found and put into variable

            GameObject FramerateObject = SettingsPanel.transform.Find("FramerateCap").gameObject;
            FramerateObject.SetActive(false); //Hide the dropdown

            //Creation and positioning of slider
            Slider = SettingsPanel.transform.Find("MasterVolume").gameObject;
            Slider = GameObject.Instantiate(Slider); //Created copy of MasterVolume, this will be as slider for the FPS
            GameObject SliderInSlider = Slider.transform.Find("Slider").gameObject;
            Object.Destroy(SliderInSlider.GetComponent<SettingsOption>()); //Remove SettingsOption component, add custom functionality
            Slider.transform.name = "FramerateSlider"; //No real use, just looks better
            Slider.transform.SetParent(SettingsPanel.transform); //Parent the slider to the SettingsPanel
            Slider.transform.position = FramerateObject.transform.position + new Vector3(-1.5f, 1f, 0f); //Position the slider to slightly above where the dropdown would be
            Slider.transform.localScale = new Vector3(1f, 1f, 1f); //Change size of the slider (and text)
            Slider.transform.Find("Image").localPosition = new Vector3(-53.4f,0f,0f); //Offset to a cap of 60
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = Initialise.setCorrectText(Slider);

            //Set values of the slider
            SliderInSlider.GetComponent<Slider>().minValue = 0; //0 = VSync
            SliderInSlider.GetComponent<Slider>().maxValue = 501; //250 = unlimited in vanilla, if set to 501 actually set to -1 (unlimited)
            SliderInSlider.GetComponent<Slider>().value = (float)IngamePlayerSettingsPatch.UnsavedLimit;

            //Add listener to slider, giving it it's functionality
            SliderInSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { Initialise.SliderValueChanged(SettingsPanel, Slider); });
        }
    }
}
