using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HomeStatsManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _totalHSText;
    [SerializeField] private TextMeshProUGUI _easyHSText;
    [SerializeField] private TextMeshProUGUI _mediumHSText;
    [SerializeField] private TextMeshProUGUI _hardHSText;
    [SerializeField] private TextMeshProUGUI _totalLevelsCText;
    [SerializeField] private TextMeshProUGUI _easyLevelsCText;
    [SerializeField] private TextMeshProUGUI _mediumLevelsCText;
    [SerializeField] private TextMeshProUGUI _hardLevelsCText;
    
    private const string currentScoreText = "Curr. Score";

    void Start() => UpdateStatsTexts();

    private void UpdateStatsTexts() {
        UpdateScoresTexts();
        UpdateLevelsCompletedTexts();
    }
    
    private void UpdateScoresTexts() {
        this._currentScoreText.text = $"{currentScoreText}: {ScoreManager.instance.GetTotalCurrentScore():F2}";
        this._totalHSText.text = $"{ScoreManager.instance.GetTotalHighScore():F2}";
        this._easyHSText.text = $"{ScoreManager.instance.easyHighScore:F2}";
        this._mediumHSText.text = $"{ScoreManager.instance.mediumHighScore:F2}";
        this._hardHSText.text = $"{ScoreManager.instance.hardHighScore:F2}";
    }

    private void UpdateLevelsCompletedTexts() {
        this._totalLevelsCText.text = $"{LevelManager.instance.GetTotalLevelsCompleted()/LevelManager.instance.GetTotalLevels():F1}";
        this._easyLevelsCText.text = $"{LevelManager.instance.currentEasyLevelsCompleted/LevelManager.instance.GetMaxEasyLevels():F1}";
        this._mediumLevelsCText.text = $"{LevelManager.instance.currentMediumLevelsCompleted/LevelManager.instance.GetMaxMediumLevels():F1}";
        this._hardLevelsCText.text = $"{LevelManager.instance.currentHardLevelsCompleted/LevelManager.instance.GetMaxHardLevels():F1}";
    }
}
