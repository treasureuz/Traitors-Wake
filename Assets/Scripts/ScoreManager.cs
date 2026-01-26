using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;
    
    private const float lowTimePoints = 10f;
    private const float middleTimePoints = 15f;
    private const float highTimePoints = 20f;
    private const float traitorKillPoints = 235f;
    private const float penaltyPoints = 80f;

    public float easyHighScore { get; private set; }
    public float mediumHighScore { get; private set; }
    public float hardHighScore { get; private set; }
    private float _easyCurrentScore;
    private float _mediumCurrentScore;
    private float _hardCurrentScore;
    
    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //void Start() => SetHighScoreByDiff(0f); // Reset high score one time
    
    // This is done by difficulty
    public void CalculateScores() {
        // HighTimeToComplete range = (CalculateHighTimeToComplete, GetTimeToComplete]
        // MidTimeToComplete range = [CalculateLowTimeToComplete, CalculateHighTimeToComplete]
        // LowTimeToComplete range = [0, CalculateLowTimeComplete)
        var currentTimeToComplete = GameManager.instance.timeToComplete;
        if (currentTimeToComplete > CalculateHighTimeToComplete()) {
            SetCurrentScoreByDiff(GetCurrentScoreByDiff() + highTimePoints * currentTimeToComplete);
        } else if (currentTimeToComplete >= CalculateLowTimeToComplete() && currentTimeToComplete <= CalculateHighTimeToComplete()) {
            SetCurrentScoreByDiff(GetCurrentScoreByDiff() + middleTimePoints * currentTimeToComplete);
        } else {
            if (currentTimeToComplete >= 0f && currentTimeToComplete < CalculateLowTimeToComplete()) {
                SetCurrentScoreByDiff(GetCurrentScoreByDiff() + lowTimePoints * currentTimeToComplete);
            }
        }
        if (GameManager.instance.traitor.isShipDestroyed || GameManager.instance.traitor.isDead) 
            SetCurrentScoreByDiff(GetCurrentScoreByDiff() + traitorKillPoints); 
        if (GetCurrentScoreByDiff() > GetHighScoreByDiff()) SetHighScoreByDiff(GetCurrentScoreByDiff());
    }

    // On Restart or Exit
    public void HandleScorePenalty() {
        // *"isPaused" check might be unnecessary since this method is only called in their two places*
        // Subtracts 200 points off current score
        if (GameManager.isPaused) SetCurrentScoreByDiff(GetCurrentScoreByDiff() - penaltyPoints);
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
    
    public void ResetCurrentScore() {
        this._easyCurrentScore = 0f;
        this._mediumCurrentScore = 0f;
        this._hardCurrentScore = 0f; 
    }
    
    public float GetCurrentScoreByDiff() {
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
    public float GetPenaltyPoints() => penaltyPoints;
}
