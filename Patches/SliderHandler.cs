using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static FramerateSlider.Initialise;
using static FramerateSlider.Patches.IngamePlayerSettingsPatch;

namespace FramerateSlider.Patches
{
    public class SliderHandler
    {
        private static GameObject mSettingsPanel; //Where all the settings are found
        private static GameObject memorySlider; //Slider this mod will be using
        private static GameObject mSliderInSlider;
        private static GameObject FramerateObject;

        private static GameObject sceneSettingsPanel;
        private static Vector3 positionOffset;
        private static bool ExistsInMemory = false;
        public static GameObject sceneSlider;
        public static GameObject sceneSliderInSlider;
        public static TMP_Text sceneSliderText;
        public static bool ignoreSliderAudio;

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
            FramerateObject = mSettingsPanel.transform.Find("FramerateCap").gameObject;
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
            return;
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
                positionOffset = new Vector3(-0.08f, 0.055f, 0f);
            }
            sceneSlider.transform.SetParent(sceneSettingsPanel.transform);
            sceneSlider.transform.SetAsFirstSibling();


            FramerateObject.SetActive(false); //Hide the dropdown

            sceneSlider.transform.localScale = new Vector3(1f, 1f, 1f);
            sceneSlider.transform.position = FramerateObject.transform.position + positionOffset;
            sceneSliderInSlider = sceneSlider.transform.Find("Slider").gameObject;
            sceneSliderInSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { SliderValueChanged(); });
            sceneSliderText = sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>();
            sceneSliderText.text = setCorrectText();
            sceneSliderInSlider.GetComponent<Slider>().value = (float)ModSettings.FramerateLimit.Value;
            ignoreSliderAudio = false;

            playerSettingsInstance.DiscardChangedSettings(); //To avoid issues
            return;
        }

        private static bool IsInMainMenu()
        {
            if (SceneManager.GetSceneByName("SampleSceneRelay").isLoaded)
            {
                return false;

            }
            return true;
        }

        private static void SliderValueChanged()
        {
            sceneSliderText.text = setCorrectText();
            TextMeshProUGUI HeaderComponent = sceneSettingsPanel.transform.Find("Headers").gameObject.transform.Find("ChangesNotApplied").gameObject.GetComponent<TextMeshProUGUI>();
            HeaderComponent.enabled = true;
            TextMeshProUGUI TextComponent = sceneSettingsPanel.transform.Find("BackButton").gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
            if (HeaderComponent.enabled)
                if (IsInMainMenu())
                {
                    TextComponent.text = "DISCARD"; //In Main Menu
                }
                else
                {
                    TextComponent.text = "Discard changes"; //In Quick Menu
                }

            if (!ignoreSliderAudio)
            {
                playerSettingsInstance.SettingsAudio.PlayOneShot(GameNetworkManager.Instance.buttonTuneSFX);
            }
            playerSettingsInstance.changesNotApplied = true;
            return;
        }
        public static string setCorrectText()
        {
            UnsavedLimit = (int)sceneSliderInSlider.GetComponent<Slider>().value;
            StringBuilder returnText = new StringBuilder("Frame rate cap: ");
            if ((int)sceneSliderInSlider.GetComponent<Slider>().value > 500)
            {
                return returnText.Append("Unlimited").ToString();
            }
            else if ((int)sceneSliderInSlider.GetComponent<Slider>().value <= 0)
            {
                return returnText.Append("VSync").ToString();
            }

            return returnText.Append(UnsavedLimit).ToString();
        }
    }
}
