using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiffSelectUIManager : MonoBehaviour {
    [SerializeField] private Button _easyGoButton;
    [SerializeField] private Button _mediumGoButton;
    [SerializeField] private Button _hardGoButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _resetAllButton;
    [SerializeField] private TextMeshProUGUI _easyLevelsText;
    [SerializeField] private TextMeshProUGUI _mediumLevelsText;
    [SerializeField] private TextMeshProUGUI _hardLevelsText;
    [SerializeField] private TextMeshProUGUI _easyHSText;
    [SerializeField] private TextMeshProUGUI _mediumHSText;
    [SerializeField] private TextMeshProUGUI _hardHSText;

    private const string levelsText = "Levels";
    private const string highScoreText = "High Score";

    void Awake() {
        this._easyGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Easy));
        this._mediumGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Medium));
        this._hardGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Hard));
    }
    
    void Start() {
        AdjustButtonsSettings(); DisableGoButtonsOnCompleted(); 
        UpdateTexts(); // Updates levels and high score texts
    }

    private void OnGo(GameManager.Difficulty difficulty) {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(difficulty);
        SceneManager.LoadScene("GameScene");
    }

    public void OnBack() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene("HomeScene");
    }

    public void OnResetAll() {
        LevelManager.hasResetRun = true; // Calls ResetRunState on Go
        LevelManager.instance.ResetAllLevels(); // Sets isCurrent{difficulty}Completed to false
        Start(); // Calls AdjustButtonsSettings and UpdateTextsByDiff
    }
    
    private void AdjustButtonsSettings() { 
        // Can only play medium if easy is completed 
        this._mediumGoButton.interactable = LevelManager.instance.isCurrentEasyCompleted;
        // Can only play hard if easy and medium is completed
        this._hardGoButton.interactable = LevelManager.instance.isCurrentMediumCompleted;
        // Can only reset all levels if easy, medium, and hard is completed
        this._resetAllButton.interactable = LevelManager.instance.isCurrentHardCompleted;
    }
        
    // ReSharper disable once InvertIf
    private void DisableGoButtonsOnCompleted() {
        if (LevelManager.instance.isCurrentEasyCompleted) this._easyGoButton.interactable = false;
        if (LevelManager.instance.isCurrentMediumCompleted) this._mediumGoButton.interactable = false;
        if (LevelManager.instance.isCurrentHardCompleted) this._hardGoButton.interactable = false;
    }

    private void UpdateTexts() {
        this._easyLevelsText.text =
            $"{levelsText}: {LevelManager.instance.currentEasyLevelsCompleted}/{LevelManager.instance.GetMaxEasyLevels()}";
        this._easyHSText.text = $"{highScoreText}: {ScoreManager.instance.GetHighScoreByDiff():F2}";
        this._mediumLevelsText.text =
            $"{levelsText}: {LevelManager.instance.currentMediumLevelsCompleted}/{LevelManager.instance.GetMaxMediumLevels()}";
        this._mediumHSText.text = $"{highScoreText}: {ScoreManager.instance.GetHighScoreByDiff():F2}";
        this._hardLevelsText.text =
            $"{levelsText}: {LevelManager.instance.currentHardLevelsCompleted}/{LevelManager.instance.GetMaxHardLevels()}";
        this._hardHSText.text = $"{highScoreText}: {ScoreManager.instance.GetHighScoreByDiff():F2}";
        
    }
}
