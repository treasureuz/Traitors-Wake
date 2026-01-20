using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;
    
    private const float lowTimePoints = 10f;
    private const float middleTimePoints = 15f;
    private const float highTimePoints = 20f;

    private float _easyCurrentScore;
    private float _mediumCurrentScore;
    private float _hardCurrentScore;
    public float easyHighScore { get; private set; }
    public float mediumHighScore { get; private set; }
    public float hardHighScore { get; private set; }
    
    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() => SetHighScoreByDiff(0f); // Reset high score one time
    
    // This is done by difficulty
    public void CalculateScores() {
        var currentTimeToComplete = GameManager.instance.timeToComplete;
        // LowTimeToComplete range = [0, CalculateLowTimeComplete)
        // MidTimeToComplete range = [CalculateLowTimeToComplete, CalculateHighTimeToComplete]
        // HighTimeToComplete range = (CalculateHighTimeToComplete, GetTimeToComplete]
        if (currentTimeToComplete >= 0f && currentTimeToComplete < CalculateLowTimeToComplete()) {
            SetCurrentScoreByDiff(GetCurrentScoreByDiff() + lowTimePoints * currentTimeToComplete);
        } else if (currentTimeToComplete >= CalculateLowTimeToComplete() &&
                   currentTimeToComplete <= CalculateHighTimeToComplete()) {
            SetCurrentScoreByDiff(GetCurrentScoreByDiff() + middleTimePoints * currentTimeToComplete);
        } else {
            if (currentTimeToComplete > CalculateHighTimeToComplete() &&
                currentTimeToComplete <= GameManager.instance.GetTimeToCompleteByDiff()) {
                SetCurrentScoreByDiff(GetCurrentScoreByDiff() + highTimePoints * currentTimeToComplete);
            }
        }
        if (GetCurrentScoreByDiff() > GetHighScoreByDiff()) SetHighScoreByDiff(GetCurrentScoreByDiff());
    }

    private static float CalculateLowTimeToComplete() => GameManager.instance.GetTimeToCompleteByDiff() / 4;
    private static float CalculateHighTimeToComplete() {
        return (GameManager.instance.GetTimeToCompleteByDiff() + GameManager.instance.GetTimeToCompleteByDiff() / 2) / 2;
    }

    public void SetCurrentScoreByDiff(float score) {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this._easyCurrentScore = score; break;
            case GameManager.Difficulty.Medium: this._mediumCurrentScore = score; break;
            case GameManager.Difficulty.Hard: this._hardCurrentScore = score; break;
        }
    }
    
    private float GetCurrentScoreByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this._easyCurrentScore,
            GameManager.Difficulty.Medium => this._mediumCurrentScore,
            GameManager.Difficulty.Hard => this._hardCurrentScore,
            _ => 0f
        };
    }
    
    private void SetHighScoreByDiff(float score) {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this.easyHighScore = score; break;
            case GameManager.Difficulty.Medium: this.mediumHighScore = score; break;
            case GameManager.Difficulty.Hard: this.hardHighScore = score; break;
        }
    }
    
    private float GetHighScoreByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this.easyHighScore,
            GameManager.Difficulty.Medium => this.mediumHighScore,
            GameManager.Difficulty.Hard => this.hardHighScore,
            _ => 0f
        };
    }
    // Total scores used in GameUIManager
    public float GetTotalCurrentScore() => this._easyCurrentScore + this._mediumCurrentScore + this._hardCurrentScore;
    public float GetTotalHighScore() => this.easyHighScore + this.mediumHighScore + this.hardHighScore;
}
