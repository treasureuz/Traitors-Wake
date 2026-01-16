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
    [SerializeField] private Button _hardResetButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _easyLevelsText;
    [SerializeField] private TextMeshProUGUI _mediumLevelsText;
    [SerializeField] private TextMeshProUGUI _hardLevelsText;

    private const string levelsText = "Levels";

    void Start() {
        this._easyGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Easy));
        this._mediumGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Medium));
        this._hardGoButton.onClick.AddListener(() => OnGo(GameManager.Difficulty.Hard));
        AdjustNonEasyGoButtons(); SetButtonsOnCompleted(); DisplayLevelsTextByDiff();
    }

    private void OnGo(GameManager.Difficulty difficulty) {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(difficulty);
        if (LevelManager.instance.GetCurrentLevelByDiff() == 0) LevelManager.hasResetRun = true;
        SceneManager.LoadScene("GameScene");
    }

    public void OnEasyReset() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(GameManager.Difficulty.Easy); // set difficulty to Easy
        LevelManager.instance.ResetCurrentLevelByDiff(); DisplayLevelsTextByDiff();
        this._mediumResetButton.interactable = false;
    }
    
    public void OnMediumReset() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(GameManager.Difficulty.Medium); // set difficulty to Medium
        LevelManager.instance.ResetCurrentLevelByDiff(); DisplayLevelsTextByDiff();
        this._mediumResetButton.interactable = false;
    }
    
    public void OnHardReset() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        GameManager.instance.SetDifficulty(GameManager.Difficulty.Hard); // set difficulty to Hard
        LevelManager.instance.ResetCurrentLevelByDiff(); DisplayLevelsTextByDiff();
        this._hardResetButton.interactable = false;
    }

    public void OnBack() {
        EventSystem.current.SetSelectedGameObject(null); // Removes "selectedButtonColor"
        SceneManager.LoadScene("HomeScene");
    }
    
    private void AdjustNonEasyGoButtons() {
            // Only can play medium if easy is completed, 
            // Only can play hard if medium is completed
            this._mediumGoButton.interactable = LevelManager.instance.isCurrentEasyCompleted;
            this._hardGoButton.interactable = LevelManager.instance.isCurrentMediumCompleted;
        }
        
    // ReSharper disable once InvertIf
    private void SetButtonsOnCompleted() {
        if (LevelManager.instance.isCurrentEasyCompleted) {
            this._easyGoButton.interactable = false;
            this._easyResetButton.interactable = true;
        }
        if (LevelManager.instance.isCurrentMediumCompleted) {
            this._mediumGoButton.interactable = false;
            this._mediumResetButton.interactable = true;
        }
        if (LevelManager.instance.isCurrentHardCompleted) {
            this._hardGoButton.interactable = false;
            this._hardResetButton.interactable = true;
        }
    }

    private void DisplayLevelsTextByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: {
                this._easyLevelsText.text =
                    $"{levelsText}: {LevelManager.instance.GetCurrentLevelByDiff()}/{LevelManager.instance.GetMaxLevelByDiff()}";
                break;
            }
            case GameManager.Difficulty.Medium: {
                this._mediumLevelsText.text =
                    $"{levelsText}: {LevelManager.instance.GetCurrentLevelByDiff()}/{LevelManager.instance.GetMaxLevelByDiff()}";
                break;
            }
            case GameManager.Difficulty.Hard: {
                this._hardLevelsText.text =
                    $"{levelsText}: {LevelManager.instance.GetCurrentLevelByDiff()}/{LevelManager.instance.GetMaxLevelByDiff()}";
                break;
            }
        }
    }
}
