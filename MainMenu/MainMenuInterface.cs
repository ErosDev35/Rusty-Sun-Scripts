using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AYellowpaper.SerializedCollections
{
    public class MainMenuInterface : MonoBehaviour
    {
        [SerializeField] Transform tabSelector;
        [SerializeField] Transform optionTabSelector;
        [SerializeField] List<Transform> underlineOfButton;
        [SerializeField] int mainTabIndex = 0;
        [SerializeField] int optionsTabIndex = 1;
        [SerializeField] List<Transform> volumeSelector;
        [SerializeField] List<Transform> simpleSliderLabels;
        [SerializeField] List<String> simpleSliderLabelsExtra;
        [SerializeField] TextMeshProUGUI gameVersion;
        [SerializedDictionary("Item Name", "Index")]
        [SerializeField] SerializedDictionary<string, GameObject> saveStateDict;
        [SerializeField] GameInterface gameInterface;
        [SerializeField] Transform cursorTrans;
        private float language = 1;
        private List<string> keyBindings;
        string saveFile;
        string worldFiles;
        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            initLoading();
        }
        void Update()
        {
            UnderlineGestion();
            SoundInterface();
            Interface();
            if (cursorTrans) CursorTransform();
        }
        void initLoading()
        {
            saveFile = Application.persistentDataPath + "/gameoptions.data";
            worldFiles = Application.persistentDataPath + "/world";

            if (File.Exists(saveFile))
            {
                print("Fichier option trouvé ! ");
                LoadOptionData(JsonUtility.FromJson<OptionData>(File.ReadAllText(saveFile)));
            }
            else
            {
                print("Fichier option non trouvé, création d'un fichier.. ");
                LoadOptionData(WriteNewOptionData());
            }
        }
        OptionData WriteNewOptionData(OptionData optionData = null)
        {
            File.WriteAllText(saveFile, JsonUtility.ToJson((optionData != null) ? optionData : new OptionData()));
            return JsonUtility.FromJson<OptionData>(File.ReadAllText(saveFile));
        }
        WorldData WriteNewWorldData(WorldData worldData = null)
        {
            File.WriteAllText(worldFiles + worldData.worldId + ".data", JsonUtility.ToJson((worldData != null) ? worldData : new WorldData()));
            print("Monde Sauvegardé ! ");
            return JsonUtility.FromJson<WorldData>(File.ReadAllText(saveFile));
        }
        void LoadOptionData(OptionData optionData)
        {
            var type = optionData.GetType();
            string consoleLogStr = "";

            foreach (var state in saveStateDict)
            {
                var option = type.GetField(state.Key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                object optionDataValue = option.GetValue(optionData);

                GameObject optionSelector = state.Value;
                float optionValue = (float)optionDataValue;

                if (optionSelector.GetComponent<Slider>()) optionSelector.GetComponent<Slider>().SetValueWithoutNotify(optionValue);
                if (optionSelector.GetComponent<Toggle>()) optionSelector.GetComponent<Toggle>().SetIsOnWithoutNotify((optionValue == 0) ? false : true);
                if (optionSelector.GetComponent<TMP_Dropdown>()) optionSelector.GetComponent<TMP_Dropdown>().SetValueWithoutNotify((int)optionValue);

                consoleLogStr += state.Key + " : " + optionDataValue + "; ";
            }
            print(consoleLogStr);
        }
        public void SaveCurrentState()
        {
            print("Save initiée");
            OptionData optionData = new OptionData();
            optionData.language = language;
            var type = optionData.GetType();
            string consoleLogStr = "";

            foreach (var state in saveStateDict)
            {
                float value = 0;

                value = (state.Value.GetComponent<Slider>()) ? Mathf.Round(state.Value.GetComponent<Slider>().value * 10000) / 10000 : value;
                value = (state.Value.GetComponent<Toggle>() && state.Value.GetComponent<Toggle>().isOn) ? 1 : value;
                value = (state.Value.GetComponent<TMP_Dropdown>()) ? state.Value.GetComponent<TMP_Dropdown>().value : value;

                consoleLogStr += state.Key + " : " + value + "; ";

                var option = type.GetField(state.Key, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                option.SetValue(optionData, value);
            }
            WriteNewOptionData(optionData);
            print(consoleLogStr);
            if (gameInterface) gameInterface.OptionInit(optionData);
            OptionReloadApplication(optionData);
        }
        void OptionReloadApplication(OptionData optionToLoad)
        {
            Vector2Int resolution = optionToLoad.screenResolutionDict[(int)optionToLoad.screenResolution];
            Screen.SetResolution(resolution.x, resolution.y, optionToLoad.screenModeDict[(int)optionToLoad.screenMode]);
        }
        void UnderlineGestion()
        {
            foreach (Transform underline in underlineOfButton)
            {
                float distance = 75 / Vector2.Distance(Input.mousePosition, underline.parent.transform.position);
                Vector3 underlineScalingVector = (distance >= 0.8f) ? new Vector3(1, 1, 1) * distance : new Vector3(0, 1, 1);
                underlineScalingVector = new Vector3(Mathf.Clamp(underlineScalingVector.x, 0f, 1), 1, 1);

                Color underlineColor = underline.GetComponent<Image>().color;
                underlineColor.a = Mathf.Clamp(distance - 0.1f, 0f, 1);

                underline.GetComponent<Image>().color = underlineColor;
                underline.localScale = underlineScalingVector;
            }
            foreach (Transform tab in tabSelector)
            {
                tab.gameObject.SetActive(tab.GetSiblingIndex() == mainTabIndex);
            }
            foreach (Transform tab in optionTabSelector)
            {
                if (tab.GetSiblingIndex() != 0)
                    tab.GetChild(1).gameObject.SetActive(tab.GetSiblingIndex() == optionsTabIndex);
            }
        }
        public void DestroySave(int saveIndex)
        {
            File.Delete(worldFiles + saveIndex + ".data");
        }
        public void DestroyAllSave()
        {
            int[] saveIndex = { 1, 2, 3 };
            foreach (int index in saveIndex)
                File.Delete(worldFiles + saveIndex + ".data");
        }
        public void SetLanguage(int language)
        {
            this.language = language;
            SaveCurrentState();
        }
        public void ResumeGame()
        {
            gameInterface.gamePaused = false;
        }
        void Interface()
        {
            if(gameVersion)
            gameVersion.text = "V." + Application.version;
            foreach(Transform label in simpleSliderLabels)
            {
                label.GetComponent<TextMeshProUGUI>().text = label.parent.GetChild(0).GetComponent<Slider>().value.ToString() + simpleSliderLabelsExtra[simpleSliderLabels.IndexOf(label)];
            }
        }
        void SoundInterface()
        {
            foreach (Transform selector in volumeSelector)
            {
                selector.GetChild(4).GetComponent<TextMeshProUGUI>().text = (Mathf.Round(selector.GetComponent<Slider>().value * 10000) / 100).ToString() + "%";
            }
        }
        public void SelectMainTab(int index)
        {
            this.mainTabIndex = index;
        }
        public void SelectOptionTab(int index)
        {
            this.optionsTabIndex = index;
        }
        public void ExitGame()
        {
            print("Goodbye");
            Application.Quit();
        }
        public void PlaySave(int index)
        {
            print("Save selected ! " + index);
            SceneManager.LoadScene(sceneBuildIndex: 1);
        }
        public void LoadMainMenu()
        {
            Time.timeScale = 1;
            gameInterface.gamePaused = false;
            SceneManager.LoadScene(0);
        }
        public void SaveWorld(int worldId)
        {
            Transform player = GameObject.Find("Player").transform;
            GameObject[] itemsToSave = GameObject.FindGameObjectsWithTag("Item");
            int itemSaveIndex = 0;

            WorldData worldData = new WorldData();
            worldData.worldId = worldId;
            worldData.playerSavedPos = player.position;
            worldData.playerSavedRotation = player.rotation * Vector3.forward;
            worldData.playerCharacterController = player.GetComponent<PlayerCharacterController>();

            foreach (GameObject item in itemsToSave)
            {
                Item itemComponent = item.GetComponent<Item>();
                GameObject itemPrefab = itemComponent.itemPrefab;

                worldData.itemsSaved.Insert(itemSaveIndex, itemPrefab);
                worldData.itemSavedPos.Insert(itemSaveIndex, item.transform.position);
                worldData.itemSavedNumber.Insert(itemSaveIndex, itemComponent.itemNumber);

                itemSaveIndex += 1;
            }
            print(worldData.itemSavedPos);
            WriteNewWorldData(worldData);

            this.mainTabIndex = 0;
            gameInterface.gamePaused = false;
        }
        public void LoadWorld(int worldId)
        {
            WorldData worldData = JsonUtility.FromJson<WorldData>(File.ReadAllText(worldFiles + worldId + ".data"));
            print(worldData.worldId);

            foreach (GameObject itemPrefab in worldData.itemsSaved)
            {
                int index = worldData.itemsSaved.IndexOf(itemPrefab);

                Vector3 itemSavedPos = worldData.itemSavedPos[index];
                print(itemPrefab.GetComponent<Item>().itemName + " " + itemSavedPos);
                var itemLoaded = Instantiate(itemPrefab);

                Item loadedItem = itemLoaded.GetComponent<Item>();
                loadedItem.itemNumber = worldData.itemSavedNumber[index];
                itemLoaded.transform.position = itemSavedPos;
            }

            this.mainTabIndex = 0;
            gameInterface.gamePaused = false;
        }
        public void CursorTransform()
        {
            Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            position.z = 0;
            cursorTrans.transform.localPosition = position;
        }
    }
}
