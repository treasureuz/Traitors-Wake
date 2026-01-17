using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;
    
    private const float lowTimePoints = 10f;
    private const float middleTimePoints = 15f;
    private const float highTimePoints = 20f;

    private float _currentEasyScore;
    private float _currentMediumScore;
    private float _currentHardScore;
    private float _easyHighScore;
    private float _mediumHighScore;
    private float _hardHighScore;
    
    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() => SetHighScoreByDiff(0f); // Reset high score one time
    
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
            case GameManager.Difficulty.Easy: this._currentEasyScore = score; break;
            case GameManager.Difficulty.Medium: this._currentMediumScore = score; break;
            case GameManager.Difficulty.Hard: this._currentHardScore = score; break;
        }
    }
    
    private void SetHighScoreByDiff(float score) {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this._easyHighScore = score; break;
            case GameManager.Difficulty.Medium: this._mediumHighScore = score; break;
            case GameManager.Difficulty.Hard: this._hardHighScore = score; break;
        }
    }
    
    private float GetCurrentScoreByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this._currentEasyScore,
            GameManager.Difficulty.Medium => this._currentMediumScore,
            GameManager.Difficulty.Hard => this._currentHardScore,
            _ => 0f
        };
    }
    public float GetTotalCurrentScore() => this._currentEasyScore + this._currentMediumScore + this._currentHardScore;
    public float GetTotalHighScore() => this._easyHighScore + this._mediumHighScore + this._hardHighScore;
    public float GetHighScoreByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this._easyHighScore,
            GameManager.Difficulty.Medium => this._mediumHighScore,
            GameManager.Difficulty.Hard => this._hardHighScore,
            _ => 0f
        };
    }
}
