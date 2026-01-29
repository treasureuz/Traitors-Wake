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
    [SerializeField] private TextMeshProUGUI _scoreCalculationText;
    
    private const string currentScoreText = "Curr. Score";
    private const string scoreCalcText = "Higher completion time = higher points.";
    private const string penaltyCalcText = "Restarting or exiting a level = ";
    private const string traitorCalcText = "Destroying the Traitor, his ship's stern, or the tiller = ";

    void Start() => UpdateStatsTexts();

    private void UpdateStatsTexts() {
        UpdateScoresTexts();
        UpdateLevelsCompletedTexts();
        DisplayScoreCalculationText();
    }
    
    private void UpdateScoresTexts() {
        this._currentScoreText.text = $"{currentScoreText}: {ScoreManager.instance.GetTotalCurrentScore():F2}";
        this._totalHSText.text = $"{ScoreManager.instance.GetTotalHighScore():F2}";
        this._easyHSText.text = $"{ScoreManager.instance.easyHighScore:F2}";
        this._mediumHSText.text = $"{ScoreManager.instance.mediumHighScore:F2}";
        this._hardHSText.text = $"{ScoreManager.instance.hardHighScore:F2}";
    }
    
    private void DisplayScoreCalculationText() {
        this._scoreCalculationText.text = $"{scoreCalcText}\n{penaltyCalcText}" +
            $"{{-{ScoreManager.instance.GetPenaltyPoints()}}} points.\n" +
            $"{traitorCalcText}{{{ScoreManager.instance.GetTraitorDeathPoints()}}}, " +
            $"{{{ScoreManager.instance.GetSternDestroyedPoints()}}}, and " +
            $"{{{ScoreManager.instance.GetTillerDestroyedPoints()}}} points; respectively.";
    }

    private void UpdateLevelsCompletedTexts() {
        // Shows in percentage 
        this._totalLevelsCText.text = 
            $"{(float)LevelManager.instance.GetTotalLevelsCompleted()/LevelManager.instance.GetTotalLevels()*100:F1}%";
        this._easyLevelsCText.text = LevelManager.instance.isCurrentEasyCompleted ? 
            $"{(float)LevelManager.instance.GetMaxEasyLevels()/LevelManager.instance.GetMaxEasyLevels()*100:F1}%" :
            $"{(float)LevelManager.instance.currentEasyLevelsCompleted/LevelManager.instance.GetMaxEasyLevels()*100:F1}%";
        this._mediumLevelsCText.text = LevelManager.instance.isCurrentMediumCompleted ? 
            $"{(float)LevelManager.instance.GetMaxMediumLevels()/LevelManager.instance.GetMaxMediumLevels()*100:F1}%" :
            $"{(float)LevelManager.instance.currentMediumLevelsCompleted/LevelManager.instance.GetMaxMediumLevels()*100:F1}%";
        this._hardLevelsCText.text = LevelManager.instance.isCurrentHardCompleted ? 
            $"{(float)LevelManager.instance.GetMaxHardLevels()/LevelManager.instance.GetMaxHardLevels()*100:F1}%" :
            $"{(float)LevelManager.instance.currentHardLevelsCompleted/LevelManager.instance.GetMaxHardLevels()*100:F1}%";
    }
}