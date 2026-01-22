using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiffSelectUIManager : MonoBehaviour {
    [SerializeField] private Button _easyGoButton;
    [SerializeField] private Button _easyResetButton;
    [SerializeField] private Button _mediumGoButton;
    [SerializeField] private Button _mediumResetButton;
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
    private const string resetText = "[RESET!]";

    void Awake() {
        this._easyGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Easy));
        this._easyResetButton.onClick.AddListener(() => OnReset(GameManager.Difficulty.Easy));
        this._mediumGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Medium));
        this._mediumResetButton.onClick.AddListener(() => OnReset(GameManager.Difficulty.Medium));
        this._hardGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Hard));
    }
    
    void Start() {
        AdjustButtonsSettings(); UpdateTexts(); // Updates levels and high score texts
    }

    private void OnGo(GameManager.Difficulty difficulty) {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(difficulty);
        if (LevelManager.instance.GetCurrentLevelByDiff() == 0) LevelManager.hasResetRun = true; // calls ResetRunState
        SceneManager.LoadScene("GameScene");
    }

    public void OnBack() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene("HomeScene");
    }

    private void OnReset(GameManager.Difficulty diff) {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        switch (diff) {
            case GameManager.Difficulty.Easy: { // Calls ResetRunState
                LevelManager.instance.ResetCurrentLevelByDiff(GameManager.Difficulty.Easy);
                GameManager.instance.DecrementResetCount(diff); break;
            }
            case GameManager.Difficulty.Medium: { // Calls ResetRunState
                LevelManager.instance.ResetCurrentLevelByDiff(GameManager.Difficulty.Medium); 
                GameManager.instance.DecrementResetCount(diff); break;
            }
        } Start(); // Calls AdjustButtonsSettings and UpdateTextsByDiff
    }
    public void OnResetAll() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        LevelManager.instance.ResetAll(); // Sets isCurrentEasy/Medium/HardCompleted to false, calls ResetRunState
        PlayersSettingsManager.instance.ResetCurrentLivesCount(); // Should make isOutOfLives false on player Start()
        Start(); // Calls AdjustButtonsSettings and UpdateTextsByDiff
    }
    
    private void AdjustButtonsSettings() {
        // Can only play easy if easy isn't completed
        this._easyGoButton.interactable = !Player.isOutOfLives && LevelManager.instance.NextLevelExistsByDiff(GameManager.Difficulty.Easy);
        this._easyResetButton.interactable = !Player.isOutOfLives && GameManager.instance.easyResetCount > 0 && !this._easyGoButton.interactable;
        // Can only play medium if easy is completed and medium isn't completed
        this._mediumGoButton.interactable = !Player.isOutOfLives && LevelManager.instance.isCurrentEasyCompleted && 
                                            LevelManager.instance.NextLevelExistsByDiff(GameManager.Difficulty.Medium);
        this._mediumResetButton.interactable = !Player.isOutOfLives && GameManager.instance.mediumResetCount > 0 && LevelManager.instance.isCurrentEasyCompleted &&
                                               !LevelManager.instance.NextLevelExistsByDiff(GameManager.Difficulty.Medium);
        // Can only play hard if medium is completed and hard isn't completed
        this._hardGoButton.interactable = !Player.isOutOfLives && LevelManager.instance.isCurrentMediumCompleted &&
                                          LevelManager.instance.NextLevelExistsByDiff(GameManager.Difficulty.Hard);
    }

    private void UpdateTexts() {
        this._easyLevelsText.text = LevelManager.instance.isCurrentEasyCompleted
            ? $"{levelsText}: {LevelManager.instance.currentEasyLevelsCompleted}/{LevelManager.instance.GetMaxEasyLevels()} " 
              + "<color=#FFFFFF>(*C.*)</color>"
            : $"{levelsText}: {LevelManager.instance.currentEasyLevelsCompleted}/{LevelManager.instance.GetMaxEasyLevels()}";
        this._easyHSText.text = $"{highScoreText}: {ScoreManager.instance.easyHighScore:F2}";
        this._easyResetButton.GetComponentInChildren<TextMeshProUGUI>().text =
              $"{resetText} ({GameManager.instance.easyResetCount}/{GameManager.instance.GetMaxResetCount(GameManager.Difficulty.Easy)})";
        this._mediumLevelsText.text = LevelManager.instance.isCurrentMediumCompleted
            ? $"{levelsText}: {LevelManager.instance.currentMediumLevelsCompleted}/{LevelManager.instance.GetMaxMediumLevels()} " 
              + "<color=#FFFFFF>(*C.*)</color>"
            : $"{levelsText}: {LevelManager.instance.currentMediumLevelsCompleted}/{LevelManager.instance.GetMaxMediumLevels()}";
        this._mediumHSText.text = $"{highScoreText}: {ScoreManager.instance.mediumHighScore:F2}";
        this._mediumResetButton.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"{resetText} ({GameManager.instance.mediumResetCount}/{GameManager.instance.GetMaxResetCount(GameManager.Difficulty.Medium)})";
        this._hardLevelsText.text = LevelManager.instance.isCurrentHardCompleted 
            ? $"{levelsText}: {LevelManager.instance.currentHardLevelsCompleted}/{LevelManager.instance.GetMaxHardLevels()} " 
              + "<color=#FFFFFF>(*C.*)</color>"
            : $"{levelsText}: {LevelManager.instance.currentHardLevelsCompleted}/{LevelManager.instance.GetMaxHardLevels()}";
        this._hardHSText.text = $"{highScoreText}: {ScoreManager.instance.hardHighScore:F2}";
    }
}
