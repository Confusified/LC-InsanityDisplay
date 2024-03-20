using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameCapSlider.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        static GameObject SettingsPanel; //Where all the settings are found
        public static GameObject Slider; //Slider this mod will be using
        private static void SliderValueChanged()
        {
            IngamePlayerSettingsPatch.UnsavedLimit = (int)Slider.transform.Find("Slider").GetComponent<Slider>().value;
            if ((int)Slider.transform.Find("Slider").GetComponent<Slider>().value > 500)
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: Unlimited";
            }
            else if ((int)Slider.transform.Find("Slider").GetComponent<Slider>().value == 0)
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: VSync";
            }
            else
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {IngamePlayerSettingsPatch.UnsavedLimit}";
            }
            SettingsPanel.transform.Find("Headers").gameObject.transform.Find("ChangesNotApplied").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            SettingsPanel.transform.Find("BackButton").gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "DISCARD";
        }
        
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
            Slider.transform.name = "FramerateSlider"; //No real use, just looks better
            Slider.transform.SetParent(SettingsPanel.transform); //Parent the slider to the SettingsPanel
            Slider.transform.position = FramerateObject.transform.position + new Vector3(-1.5f, 1f, 0f); //Position the slider to slightly above where the dropdown would be
            Slider.transform.localScale = new Vector3(1f, 1f, 1f); //Change size of the slider (and text)
            Slider.transform.Find("Image").localPosition = new Vector3(-53.4f,0f,0f); //Offset to a cap of 60
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialize.ModSettings.FramerateLimit.Value}";
            Object.Destroy(Slider.transform.Find("Slider").GetComponent<SettingsOption>()); //Remove SettingsOption component, add custom functionality
            Slider.transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener(delegate { SliderValueChanged(); });

            //Set values of the slider
            Slider.transform.Find("Slider").GetComponent<Slider>().minValue = 0; //0 = VSync
            Slider.transform.Find("Slider").GetComponent<Slider>().maxValue = 501; //250 = unlimited in vanilla, if set to 501 actually set to -1 (unlimited)
            Slider.transform.Find("Slider").GetComponent<Slider>().value = Initialize.ModSettings.FramerateLimit.Value;
        }
    }
}
