using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject gamePanel;
    public GameObject controlsPanel;
    public GameObject videoPanel;

    public Button gamePanelButton;
    public Button controlsPanelButton;
    public Button videoPanelButton;
    public Button exitToMenuButton;

    // Video setting stuff
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;

    // Gameplay options stuff
    public TMP_InputField sensInput;
    public Slider sensSlider;
    public PlayerLook playerLook;

    // Keybind changing
    public GameObject keybindList;
    public GameObject keyListItem;
    public PlayerController playerController;
    // Start is called before the first frame update

    private string keyToRebind = null;
    private Dictionary<string, TextMeshProUGUI> buttonNames;
    private Dictionary<string, Resolution> availableResolutions;
    private int selectedResolutionIndex;

    void Start()
    {
        if (!PlayerPrefs.HasKey("resIndex"))
        {
            PlayerPrefs.SetInt("resIndex", 0);
            PlayerPrefs.Save();
        }

        buttonNames = new Dictionary<string, TextMeshProUGUI>();
        availableResolutions = new Dictionary<string, Resolution>();
        foreach (KeyValuePair<string, KeyCode> kvp in playerController.keybinds)
        {
            GameObject g = Instantiate(keyListItem, keybindList.transform);
            g.transform.GetChild(0);

            TextMeshProUGUI txt = g.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            txt.text = kvp.Key;

            Button keybindButton = g.transform.Find("Button").GetComponent<Button>();

            TextMeshProUGUI buttonTxt = keybindButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>();
            buttonTxt.text = kvp.Value.ToString();
            keybindButton.onClick.AddListener( () => { StartRebindActionFor(txt.text); } );

            buttonNames[txt.text] = buttonTxt;
        }

        gamePanelButton.onClick.AddListener(SwitchToGamePanel);
        controlsPanelButton.onClick.AddListener(SwitchToControlsPanel);
        videoPanelButton.onClick.AddListener(SwitchToVideoPanel);
        exitToMenuButton.onClick.AddListener(ExitToMenu);

        Resolution[] resolutions = Screen.resolutions;
        Array.Reverse(resolutions);
        foreach (Resolution res in resolutions)
        {
            if (res.refreshRate < 50) { continue; }
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.ToString()));
            availableResolutions[res.ToString()] = res;
        }

        resolutionDropdown.value = PlayerPrefs.GetInt("resIndex");

        Resolution selectedRes = availableResolutions[resolutionDropdown.options[selectedResolutionIndex].text];
        Screen.SetResolution(selectedRes.width, selectedRes.height, true, selectedRes.refreshRate);

        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChanged(); });
        windowModeDropdown.onValueChanged.AddListener(delegate { OnWindowModeChanged(); });

        sensInput.onValueChanged.AddListener(delegate { OnMouseSensInputChanged(); });
        sensSlider.onValueChanged.AddListener(delegate { OnMouseSliderMoved(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                // Update tutorial level keybind if called within tutorial
                if (SceneManager.GetActiveScene().name.Equals("Tutorial") && FindObjectOfType<TutorialTrigger>() != null)
                {
                    FindObjectOfType<TutorialTrigger>().UpdateInput();
                }
                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
            }
            else
            {
                menu.SetActive(true);
                gamePanel.SetActive(true);
                controlsPanel.SetActive(false);
                videoPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
        }

        if (menu.activeInHierarchy)
        {
            if (controlsPanel.activeInHierarchy && keyToRebind != null)
            {
                if (Input.anyKeyDown)
                {
                    KeyCode[] keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
                    foreach (KeyCode keyCode in keyCodes)
                    {
                        if (Input.GetKeyDown(keyCode) && keyCode != KeyCode.Escape)
                        {
                            playerController.keybinds[keyToRebind] = keyCode;
                            PlayerPrefs.SetString(keyToRebind, keyCode.ToString());
                            PlayerPrefs.Save();
                            buttonNames[keyToRebind].text = keyCode.ToString();
                            keyToRebind = null;
                        }
                    }
                }
            }

            if (videoPanel.activeInHierarchy)
            {
               
            }

            if (gamePanel.activeInHierarchy)
            {

            }
        }
        else
        {
            keyToRebind = null;
        }
    }

    void StartRebindActionFor(string keybindName)
    {
        keyToRebind = keybindName;
    }

    void SwitchToGamePanel()
    {
        gamePanel.SetActive(true);
        controlsPanel.SetActive(false);
        videoPanel.SetActive(false);
    }

    void SwitchToControlsPanel()
    {
        gamePanel.SetActive(false);
        controlsPanel.SetActive(true);
        videoPanel.SetActive(false);
    }

    void SwitchToVideoPanel()
    {
        gamePanel.SetActive(false);
        controlsPanel.SetActive(false);
        videoPanel.SetActive(true);
    }

    void ExitToMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    void OnResolutionChanged()
    {
        selectedResolutionIndex = resolutionDropdown.value;
        PlayerPrefs.SetInt("resIndex", selectedResolutionIndex);
        PlayerPrefs.Save();
        Resolution selectedRes = availableResolutions[resolutionDropdown.options[selectedResolutionIndex].text];
        Screen.SetResolution(selectedRes.width, selectedRes.height, true, selectedRes.refreshRate);
    }

    void OnWindowModeChanged()
    {
        if (windowModeDropdown.value == 0)
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } 
        else if (windowModeDropdown.value == 1)
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else if (windowModeDropdown.value == 2)
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    void OnMouseSensInputChanged()
    {
        if (sensInput.text.Length > 5)
        {
            sensInput.text = sensInput.text.Substring(0, 5);
        }

        if (float.TryParse(sensInput.text, out float sens))
        {
            if (sens > sensSlider.maxValue) { sens = sensSlider.maxValue; }
            if (sens < sensSlider.minValue) { sens = sensSlider.minValue; }
            sensSlider.value = sens;
        }
        else
        {
            sensInput.text = 0.5f.ToString();
        }
        playerLook.changeMouseSens(sensSlider.value);
    }

    void OnMouseSliderMoved()
    {
        sensInput.text = sensSlider.value.ToString();
        playerLook.changeMouseSens(sensSlider.value);
    }
}
