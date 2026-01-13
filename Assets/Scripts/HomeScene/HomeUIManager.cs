using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour {
    [SerializeField] private GameObject _storyUIPrefab;
    [SerializeField] private GameObject _powerUpsUIPrefab;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _controlsText;
    [SerializeField] private float _typewriterDelay = 0.55f;
    private Canvas _canvas;

    private GameObject _activePanelPrefab;
    private GameObject _spawnedPanelUI;
    private HomeButtonsBehavior[] _homeButtons;
    public static HomeUIManager instance;
    
    private const string titleText = "TRAITOR'S WAKE";
    private const string controlsText = "W, A, S, D (*Arrow keys apply*)";

    void Awake() {
        if (instance) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        this._activePanelPrefab = this._storyUIPrefab; // Starting panel: StoryUI
    }
    
    void Start() {
        this._homeButtons = this._canvas.GetComponentsInChildren<HomeButtonsBehavior>();
        SetHomeButtons(false); // Deactivate homeButtons on start
        this._titleText.text = "";
        StartCoroutine(StartTitleAnimation());
        this._controlsText.text = "<u>Controls</u>: " + controlsText; // Set controls text
    }
    
    private IEnumerator StartTitleAnimation() {
        var index = 0;
        while (index < titleText.Length) {
            yield return new WaitForSeconds(this._typewriterDelay);
            this._titleText.text += titleText[index++];
        }
        this._titleText.text = titleText;
        SetHomeButtons(true); // Reactivate buttons after title animation is done
    }
    
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        // If scene is HomeScene, find references
        if (scene.name == "HomeScene") Begin();
    }
    
    private void Begin() {
        this._canvas = FindFirstObjectByType<Canvas>();
        this._controlsText = this._canvas.transform.Find("ControlsText (TMP)").GetComponent<TextMeshProUGUI>();
        this._titleText = this._canvas.transform.Find("TitleCardBorder/TitleCard/TitleText (TMP)").GetComponent<TextMeshProUGUI>();
        DestroyAllPanelsExcept(this._activePanelPrefab); // Spawn panel
    }

    public void OnPlay() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene("DifficultySelectScene");
    }

    public void OnStory() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        DestroyAllPanelsExcept(this._storyUIPrefab); // Shows storyUI
    }
    
    public void OnPowerUps() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        DestroyAllPanelsExcept(this._powerUpsUIPrefab); // Shows powerUpsUI
    }

    private void DestroyAllPanelsExcept(GameObject prefabToInstantiate) {
        if (_spawnedPanelUI) Destroy(this._spawnedPanelUI); // Destroy previous panel if it exists
        // Instantiate the panel (prefabToInstantiate)
        this._spawnedPanelUI = Instantiate(prefabToInstantiate, _canvas.transform, false);
        this._activePanelPrefab = prefabToInstantiate;
        AddPanelsOnClickListeners();
    }

    private void AddPanelsOnClickListeners() {
        if (this._activePanelPrefab == this._storyUIPrefab) {
            Button rightButton = this._spawnedPanelUI.transform.Find("StoryBG/StoryTextBG/RightButton").GetComponent<Button>();
            rightButton.onClick.AddListener(OnPowerUps);
        } else if (this._activePanelPrefab == this._powerUpsUIPrefab) {
            Button leftButton = this._spawnedPanelUI.transform.Find("PowerUpBG/PowerUpTextBG/LeftButton").GetComponent<Button>();
            Button rightButton = this._spawnedPanelUI.transform.Find("PowerUpBG/PowerUpTextBG/RightButton").GetComponent<Button>();
            leftButton.onClick.AddListener(OnStory);
            rightButton.onClick.AddListener(OnPowerUps);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetHomeButtons(bool enable) {
        foreach (HomeButtonsBehavior button in this._homeButtons) {
            button.GetComponent<Button>().interactable = enable;
        }
    }
}
