using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    [Header("Levels Per Difficulty")] 
    [SerializeField] private int _maxEasyLevels = 4;
    [SerializeField] private int _maxMediumLevels = 3;
    [SerializeField] private int _maxHardLevels = 2;
    
    [Header("Other settings")]
    [SerializeField] private float _timeBeforeLevelStart = 1f;
    
    public static LevelManager instance;
    private Coroutine _levelCoroutine;
    
    private int _currentTotalLevel;
    private int _currentEasyLevel = 1;
    private int _currentMediumLevel = 1;
    private int _currentHardLevel = 1;
    
    public static bool hasResetRun;

    private bool _isEasyCompleted;
    private bool _isMediumCompleted;
    private bool _isHardCompleted;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        hasResetRun = true;
    }

    public void TryStartLevel() {
        // Start if player has completed all the levels for a certain difficulty
        // Else, take user back to DifficultySelectScene
        if (HasNextLevelForDifficulty()) StartLevelCoroutine(StartCoroutine(GenerateLevel()));
        else SceneManager.LoadScene("DifficultySelectScene");
    }

    private void StartLevelCoroutine(Coroutine coroutine) {
        if (this._levelCoroutine != null) StopCoroutine(this._levelCoroutine);
        this._levelCoroutine = coroutine;
    }

    private IEnumerator GenerateLevel() {
        if (hasResetRun) ResetRunState();

        // Disable UI elements and wait X amount of time before generating level
        UIManager.instance.SetActionButtons(false);
        UIManager.instance.SetPauseButton(false);
        UIManager.instance.SetOnPauseButtons(false); // Disable Resume, Restart, Home on start
        UIManager.instance.SetEndScreenButtons(false);
        yield return new WaitForSeconds(this._timeBeforeLevelStart);

        // Reset all "Player" related settings before re-generating a level
        GameManager.instance.traitor.ResetLevelSettings();
        GameManager.instance.player.ResetLevelSettings();
        GridManager.instance.ClearAllTileTypes();
        
        UIManager.instance.DisplayLevelText(GetCurrentLevelByDiff()); // Display current level
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty

        // Enables player and traitor after grid is generated
        GameManager.instance.EnablePlayers();

        // Set lineRenderer to the AI's spawn position
        GameManager.instance.traitor.SetLRPosCount(1); // Set LineRenderer position count to 1
        GameManager.instance.traitor.SetLineRendererStatus(true); // Enable LineRenderer
        GameManager.instance.traitor.SetLRPosition(0, PlayerManager.SpawnPosV3());

        // After move sequence, remove AI path trace, and enable player button actions
        UIManager.instance.SetPauseButton(true);
        GameManager.instance.traitor.StartMoveSequenceCoroutine();
        yield return new WaitUntil(() => !GameManager.instance.traitor.isMoving);
        UIManager.instance.StartTimeToMemorizeCoroutine();
        yield return new WaitUntil(() => !UIManager.instance.isMemorizing);

        // After timeToMemorize, wait an additional 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        UIManager.instance.SetActionButtons(true);
        // Disable LineRenderer if player doesn't have the power up (based on levels per difficulty)
        if (!PowerUpManager.instance.isLineTrace) GameManager.instance.traitor.SetLineRendererStatus(false);
        GameManager.instance.traitor.hasEnded = true; // Sets traitor.hasEnded to true after timeToMemorize is complete

        UIManager.instance.StartTimeToCompleteCoroutine();
        yield return new WaitUntil(() => !UIManager.instance.isCompleting);
    }

    public void DetermineNextEvent() {
        if (GameManager.instance.player.MovesEquals(GameManager.instance.traitor)) {
            // If the player beat the final level, then call EndGame -> Win Screen
            // Else, send the player to the next level
            if (this._currentTotalLevel == GetTotalLevels()) {
                Player.hasWon = true;
                EndGame(); // -> Win Screen
            } else {
                UIManager.instance.DisplayLoadingText(); // calls UpdateScoreText()
                IncrementCurrentLevelByDiff(); // Only increment if last level was completed
                TryStartLevel(); // Start next level
            }
        } else {
            Player.hasWon = false;
            hasResetRun = true;
            EndGame(); // -> Lose Screen
        }
    }
    
    private bool HasNextLevelForDifficulty() {
        switch(GameManager.instance.difficulty){
            case GameManager.Difficulty.Easy: {
                // Return true -- can advance
                if (this._isEasyCompleted) this._currentEasyLevel = 1; this._isEasyCompleted = false;
                if (this._currentEasyLevel <= this._maxEasyLevels) return true;
                // Return false -- can't advance
                this._isEasyCompleted = true; hasResetRun = true;
                return false;
            }
            case GameManager.Difficulty.Medium: {
                // Return true -- can advance
                if (this._isMediumCompleted) this._currentMediumLevel = 1; this._isMediumCompleted = false;
                if (this._currentMediumLevel <= this._maxMediumLevels) return true;
                // Return false -- can't advance
                this._isMediumCompleted = true; hasResetRun = true;
                return false;
            }
            case GameManager.Difficulty.Hard: {
                // Return true -- can advance
                if (this._isHardCompleted) this._currentHardLevel = 1; this._isHardCompleted = false;
                if (this._currentHardLevel <= this._maxHardLevels) return true;
                // Return false -- can't advance
                this._isHardCompleted = true; hasResetRun = true;
                return false;
            }
            default: return false;
        }  
    }

    private void ResetRunState() {
        StopAllCoroutines();
        // Reset everything
        ScoreManager.instance.SetCurrentScore(0f); 
        UIManager.instance.UpdateScoreText();
        hasResetRun = false; GameManager.isPaused = false;
        GameManager.instance.player.ResetPlayerSettings();
        GameManager.instance.traitor.ResetPlayerSettings();
        PowerUpManager.instance.ResetPowerUps(); // calls UIManager.DisablePowerUpSprites()
    }

    public void EndGame() {
        StopAllCoroutines();
        UIManager.instance.DisplayEndScreen();
    }

    private new void StopAllCoroutines() {
        GameManager.instance.traitor.TryStopMoveSequenceCoroutine();
        UIManager.instance.TryStopTimeToMemorizeCoroutine();
        UIManager.instance.TryStopTimeToCompleteCoroutine();
        TryStopLevelCoroutine();
    }
    
    private void TryStopLevelCoroutine() {
        if (this._levelCoroutine == null) return;
        StopCoroutine(this._levelCoroutine);
        this._levelCoroutine = null;
    }

    public int GetCurrentLevelByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this._currentEasyLevel,
            GameManager.Difficulty.Medium => this._currentMediumLevel,
            GameManager.Difficulty.Hard => this._currentHardLevel,
            _ => 0
        };
    }

    private void IncrementCurrentLevelByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this._currentEasyLevel++; break;
            case GameManager.Difficulty.Medium: this._currentMediumLevel++; break;
            case GameManager.Difficulty.Hard: this._currentHardLevel++; break;
        }
    }
    
    public int GetMaxLevelByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this._maxEasyLevels,
            GameManager.Difficulty.Medium => this._maxMediumLevels,
            GameManager.Difficulty.Hard => this._maxHardLevels,
            _ => 0
        };
    }
    
    private int GetTotalLevels() {
        return this._maxEasyLevels + this._maxMediumLevels + this._maxHardLevels;
    }
}
