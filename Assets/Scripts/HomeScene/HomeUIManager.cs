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
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _storyButton;
    [SerializeField] private Button _powerUpsButton;
    [SerializeField] private GameObject _storyUIPrefab;
    [SerializeField] private GameObject _powerUpsUIPrefab;
    //[SerializeField] private Vector3 _panelsPosition = new(-550, 0, 0);
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _controlsText;
    [SerializeField] private float _typewriterDelay = 0.55f;
    private Canvas _canvas;

    private GameObject _activePanelPrefab;
    private GameObject _spawnedPanelUI;
    public static HomeUIManager instance;
    
    private const string titleText = "TRAITOR'S WAKE";
    private const string controlsText = "W, A, S, D (*Arrow keys apply*)";
    private const string gameSceneName = "GameScene";

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
        this._titleText.text = "";
        StartCoroutine(StartTitleAnimation());
        this._controlsText.text = "<u>Controls</u>: " + controlsText; // Set controls text
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
        this._canvas = FindAnyObjectByType<Canvas>();
        this._controlsText = this._canvas.transform.Find("ControlsText (TMP)").GetComponent<TextMeshProUGUI>();
        this._titleText = this._canvas.transform.Find("TitleCardBorder/TitleCard/TitleText (TMP)").GetComponent<TextMeshProUGUI>();
        DestroyAllPanelsExcept(this._activePanelPrefab); // Spawn panel
        // Finds all button objects and puts them in an array (unsorted)
        // FirstOrDefault = Get first object with the name or default to null if name doesn't exist
        this._playButton = FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(button => button.name == "PlayButton");
        this._storyButton = FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(button => button.name == "StoryButton");
        this._powerUpsButton = FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(button => button.name == "PowerUpsButton");
    }
    
    private IEnumerator StartTitleAnimation() {
        var index = 0;
        while (index < titleText.Length) {
            yield return new WaitForSeconds(this._typewriterDelay);
            this._titleText.text += titleText[index++];
        }
        this._titleText.text = titleText;
    }

    public void OnPlay() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene(gameSceneName);
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
        // Destroy previous panel if it exists
        if (_spawnedPanelUI) Destroy(this._spawnedPanelUI);
        // Instantiate the one we want
        this._spawnedPanelUI = Instantiate(prefabToInstantiate, _canvas.transform, false);
        this._activePanelPrefab = prefabToInstantiate;
        AddPanelOnClickListeners();
    }

    private void AddPanelOnClickListeners() {
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
}
