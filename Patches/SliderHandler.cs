using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FramerateSlider.Patches
{
    public class SliderHandler
    {
        public static bool ExistsInMemory = false;

        private static GameObject mSettingsPanel; //Where all the settings are found
        private static GameObject memorySlider; //Slider this mod will be using
        private static GameObject mSliderInSlider;

        public static GameObject sceneSlider;
        private static GameObject sceneSettingsPanel;
        private static GameObject sceneSliderInSlider;
        private static Vector3 positionOffset;
        private static bool ignoreSliderAudio;

        public static void CreateSliderInMemory()
        {
            if (ExistsInMemory) { return; } //To avoid a memory leak
            if (IsInMainMenu())
            {
                mSettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject;
            }
            else
            {
                //This shouldn't be needed but oh well
                mSettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject;
            }
            GameObject FramerateObject = mSettingsPanel.transform.Find("FramerateCap").gameObject;

            //Creation and positioning of slider
            memorySlider = mSettingsPanel.transform.Find("MasterVolume").gameObject;
            memorySlider = GameObject.Instantiate(memorySlider); //Created copy of MasterVolume, this will be as slider for the FPS

            mSliderInSlider = memorySlider.transform.Find("Slider").gameObject;
            Object.DestroyImmediate(mSliderInSlider.GetComponent<SettingsOption>()); //Remove SettingsOption component, add custom functionality

            memorySlider.transform.name = "FramerateSlider";
            memorySlider.transform.position = FramerateObject.transform.position; //Position the slider to slightly above where the dropdown would be

            //Set values of the slider
            mSliderInSlider.GetComponent<Slider>().minValue = 0; //0 = VSync
            mSliderInSlider.GetComponent<Slider>().maxValue = 501; //501 == Unlimited actually set to -1 (true unlimited), vanilla unlimited would be 250
            Object.DontDestroyOnLoad(memorySlider); //Put Slider into memory
            ExistsInMemory = true;
        }

        public static void CreateSliderInScene()
        {
            if (!ExistsInMemory) { return; } //To avoid potential errors
            sceneSlider = Object.Instantiate(memorySlider);
            ignoreSliderAudio = true;
            if (IsInMainMenu())
            {
                sceneSettingsPanel = GameObject.Find("Canvas").gameObject.transform.Find("MenuContainer").gameObject.transform.Find("SettingsPanel").gameObject;
                positionOffset = new Vector3(-1.4f, 0.8f, 0f);
            }
            else
            {
                sceneSettingsPanel = GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("QuickMenu").gameObject.transform.Find("SettingsPanel").gameObject;
                positionOffset = new Vector3(-0.08f, 0.06f, 0f);
            }
            sceneSlider.transform.SetParent(sceneSettingsPanel.transform);
            sceneSlider.transform.SetAsFirstSibling();

            GameObject FramerateObject = sceneSettingsPanel.transform.Find("FramerateCap").gameObject;
            FramerateObject.SetActive(false); //Hide the dropdown

            sceneSlider.transform.localScale = new Vector3(1f, 1f, 1f);
            sceneSlider.transform.position = FramerateObject.transform.position + positionOffset;
            sceneSlider.transform.Find("Image").localPosition = new Vector3(-53.4f, 0f, 0f); //Offset to a cap of 60 (when the cursor is on it)
            sceneSliderInSlider = sceneSlider.transform.Find("Slider").gameObject;
            sceneSliderInSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { SliderValueChanged(sceneSettingsPanel, sceneSlider); });
            sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = setCorrectText(sceneSlider);
            sceneSliderInSlider.GetComponent<Slider>().value = (float)Initialise.ModSettings.FramerateLimit.Value;
            ignoreSliderAudio = false;

            IngamePlayerSettings.Instance.DiscardChangedSettings(); //To avoid issues
        }

        private static bool IsInMainMenu()
        {
            if (SceneManager.GetSceneByName("SampleSceneRelay").isLoaded)
            {
                return false;

            }
            else
            {
                return true;
            }
        }

        private static void SliderValueChanged(GameObject SettingsPanel, GameObject Slider)
        {
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = setCorrectText(Slider);
            SettingsPanel.transform.Find("Headers").gameObject.transform.Find("ChangesNotApplied").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            if (IsInMainMenu())
            {
                SettingsPanel.transform.Find("BackButton").gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "DISCARD"; //In Main Menu
            }
            else
            {
                SettingsPanel.transform.Find("BackButton").gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "Discard changes"; //In Quick Menu
            }
            if (!ignoreSliderAudio)
            {
                IngamePlayerSettings.Instance.SettingsAudio.PlayOneShot(GameNetworkManager.Instance.buttonTuneSFX);
            }
            IngamePlayerSettings.Instance.changesNotApplied = true;
        }
        private static string setCorrectText(GameObject Slider)
        {
            IngamePlayerSettingsPatch.UnsavedLimit = (int)Slider.transform.Find("Slider").GetComponent<Slider>().value;
            if ((int)Slider.transform.Find("Slider").GetComponent<Slider>().value > 500)
            {
                return "Frame rate cap: Unlimited";
            }
            else if ((int)Slider.transform.Find("Slider").GetComponent<Slider>().value <= 0)
            {
                return "Frame rate cap: VSync";
            }
            else
            {
                return $"Frame rate cap: {IngamePlayerSettingsPatch.UnsavedLimit}";
            }
        }
    }
}
