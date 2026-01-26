using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour {
    [SerializeField] private GameObject _storyUIPrefab;
    [SerializeField] private GameObject _powerUpsUIPrefab;
    [SerializeField] private GameObject _scoresUIPrefab;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _controlsText;
    [SerializeField] private float _typewriterDelay = 0.55f;
    private Canvas _canvas;

    private GameObject _activePanelPrefab;
    private GameObject _spawnedPanelUI;
    private readonly List<Button> _homeButtons = new();
    public static HomeUIManager instance;
    
    private const string titleText = "TRAITOR'S WAKE";
    private const string controlsText = "<u>Controls</u>: W, A, S, D (*Arrow keys apply*)";

    void Awake() {
        if (instance) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        this._activePanelPrefab = this._storyUIPrefab; // Starting panel: StoryUI
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
        DestroyAllPanelsExcept(this._activePanelPrefab); // Spawn active panel
    }
    
    void Start() {
        foreach (HomeButtonsBehavior hb in FindObjectsByType<HomeButtonsBehavior>(FindObjectsSortMode.None)) {
            this._homeButtons.Add(hb.GetComponent<Button>()); }
        SetHomeButtons(false); // Deactivate homeButtons on start
        this._titleText.text = "";
        StartCoroutine(StartTitleAnimation());
        this._controlsText.text = controlsText; // Set controls text
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
    
    public void OnScores() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        DestroyAllPanelsExcept(this._scoresUIPrefab); // Shows powerUpsUI
    }

    private void DestroyAllPanelsExcept(GameObject prefabToInstantiate) {
        if (this._spawnedPanelUI) Destroy(this._spawnedPanelUI); // Destroy previous panel if it exists
        // Instantiate the panel (prefabToInstantiate)
        this._spawnedPanelUI = Instantiate(prefabToInstantiate, this._canvas.transform, false);
        this._activePanelPrefab = prefabToInstantiate;
        AddPanelsOnClickListeners();
    }
    
    private void AddPanelsOnClickListeners() {
        // Make left and/or right buttons for each panel work appropriately
        if (this._activePanelPrefab == this._storyUIPrefab) { // Active panel is StoryPanel
            Button rightButton = this._spawnedPanelUI.transform.Find("StoryBG/StoryTextBG/RightButton").GetComponent<Button>();
            rightButton.onClick.AddListener(OnPowerUps);
        } else if (this._activePanelPrefab == this._powerUpsUIPrefab) { // Active panel is PowerUpsPanel
            Button leftButton = this._spawnedPanelUI.transform.Find("PowerUpBG/PowerUpTextBG/LeftButton").GetComponent<Button>();
            Button rightButton = this._spawnedPanelUI.transform.Find("PowerUpBG/PowerUpTextBG/RightButton").GetComponent<Button>();
            leftButton.onClick.AddListener(OnStory);
            rightButton.onClick.AddListener(OnScores);
        } else if (this._activePanelPrefab == this._scoresUIPrefab) { // Active panel is ScoresPanel
            Button leftButton = this._spawnedPanelUI.transform.Find("ScoresBG/ScoresTextBG/LeftButton").GetComponent<Button>();
            leftButton.onClick.AddListener(OnPowerUps);
        }
    }
    
    private void SetHomeButtons(bool enable) {
        foreach (Button button in this._homeButtons) {
            button.interactable = enable;
        }
    }
}
