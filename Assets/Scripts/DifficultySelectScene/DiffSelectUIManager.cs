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
        AdjustGoButtonsSettings(); UpdateStatsTexts(); // Updates levels and high score texts
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
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        LevelManager.hasResetRun = true; // Calls ResetRunState on GO
        LevelManager.instance.ResetAllLevels(); // Sets isCurrentEasy/Medium/HardCompleted to false
        // These two get called when the player clicks GO and LevelManager calls ResetRunState()
        // Might be considered redundant to do them again
        // GameManager.instance.SetDifficulty(GameManager.Difficulty.Easy); // Reset diff to Easy
        // ScoreManager.instance.SetCurrentScoreByDiff(0f); // Reset current score to 0
        Start(); // Calls AdjustButtonsSettings and UpdateTextsByDiff
    }
    
    private void AdjustGoButtonsSettings() {
        // Can only play easy if easy isn't completed
        this._easyGoButton.interactable = !LevelManager.instance.isCurrentEasyCompleted;
        // Can only play medium if easy is completed and medium isn't completed
        this._mediumGoButton.interactable = LevelManager.instance.isCurrentEasyCompleted && !LevelManager.instance.isCurrentMediumCompleted;
        // Can only play hard if medium is completed and hard isn't completed
        this._hardGoButton.interactable = LevelManager.instance.isCurrentMediumCompleted && !LevelManager.instance.isCurrentHardCompleted;
    }

    private void UpdateStatsTexts() {
        this._easyLevelsText.text =
            $"{levelsText}: {LevelManager.instance.currentEasyLevelsCompleted}/{LevelManager.instance.GetMaxEasyLevels()}";
        this._easyHSText.text = $"{highScoreText}: {ScoreManager.instance.easyHighScore:F2}";
        this._mediumLevelsText.text =
            $"{levelsText}: {LevelManager.instance.currentMediumLevelsCompleted}/{LevelManager.instance.GetMaxMediumLevels()}";
        this._mediumHSText.text = $"{highScoreText}: {ScoreManager.instance.mediumHighScore:F2}";
        this._hardLevelsText.text =
            $"{levelsText}: {LevelManager.instance.currentHardLevelsCompleted}/{LevelManager.instance.GetMaxHardLevels()}";
        this._hardHSText.text = $"{highScoreText}: {ScoreManager.instance.hardHighScore:F2}";
    }
}
