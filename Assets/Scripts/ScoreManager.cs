using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;
    
    private const float lowTimePoints = 10f;
    private const float middleTimePoints = 15f;
    private const float highTimePoints = 20f;

    private float _currentScore;
    private float _highScore;
    
    void Awake() {
        instance = this;
    }

    public void CalculateScores() {
        if (LevelManager.hasResetRun) return; // Don't calculate on start of every level
        var currentTimeToComplete = GameManager.instance.timeToComplete;
        // LowTimeToComplete range = [0, CalculateLowTimeComplete)
        // MidTimeToComplete range = [CalculateLowTimeToComplete, CalculateHighTimeToComplete]
        // HighTimeToComplete range = (CalculateHighTimeToComplete, GetTimeToComplete]
        if (currentTimeToComplete >= 0f && currentTimeToComplete < CalculateLowTimeToComplete()) {
            this._currentScore += lowTimePoints * currentTimeToComplete;
        } else if (currentTimeToComplete >= CalculateLowTimeToComplete() &&
                   currentTimeToComplete <= CalculateHighTimeToComplete()) {
            this._currentScore += middleTimePoints * currentTimeToComplete;
        } else {
            if (currentTimeToComplete > CalculateHighTimeToComplete() &&
                currentTimeToComplete <= GameManager.instance.GetTimeToComplete()) {
                this._currentScore += highTimePoints * currentTimeToComplete;
            }
        }
        if (this._currentScore > this._highScore) this._highScore = this._currentScore;
    }

    private static float CalculateLowTimeToComplete() => GameManager.instance.GetTimeToComplete() / 4;
    private static float CalculateHighTimeToComplete() {
        return (GameManager.instance.GetTimeToComplete() + GameManager.instance.GetTimeToComplete() / 2) / 2;
    }
    
    public void SetCurrentScore(float score) => this._currentScore = score;
    public float GetCurrentScore() => this._currentScore;
    public float GetHighScore() => this._highScore;
}
