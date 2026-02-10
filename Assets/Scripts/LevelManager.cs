using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    [Header("Levels Per Difficulty")] 
    [SerializeField] private int _maxEasyLevels = 4;
    [SerializeField] private int _maxMediumLevels = 3;
    [SerializeField] private int _maxHardLevels = 2;
    
    [Header("Other Settings")]
    [SerializeField] private float _timeBeforeLevelStart = 1f;
    
    private Coroutine _levelCoroutine;

    public static LevelManager instance;
    public static bool hasResetRun;
    
    private int _totalLevelsCompleted;
    public int currentEasyLevelsCompleted { get; private set; }
    public int currentMediumLevelsCompleted { get; private set; }
    public int currentHardLevelsCompleted { get; private set; }
    public bool isEasyCompleted { get; private set; }
    public bool isMediumCompleted { get; private set; }
    public bool isHardCompleted { get; private set; }
    public bool isDifficultyComplete { get; set; }
    public bool isLevelComplete {get; set;}

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        hasResetRun = true;
    }
    
    private void ResetRunState() {
        StopAllCoroutines(true);
        // Reset everything
        hasResetRun = false; GameManager.isPaused = false;
        UIManager.instance.UpdateScoreText(); // Doesn't calculate score
        GameManager.instance.player.ResetPlayerSettings();
        GameManager.instance.traitor.ResetPlayerSettings();
        PlayersSettingsManager.instance.SavePlayersSettings();
        PlayersSettingsManager.instance.ApplyPlayersSettings(); // Apply saved players data/settings
        // Reset power ups and display the change
        GameManager.instance.GetPowerUpManagerByDiff().ResetPowerUpsSettings(); 
        UIManager.instance.UpdatePowerUpsUI();
    }
    
    public void TryStartLevel() {
        // Setup everything before starting
        if (hasResetRun) ResetRunState(); 
        // Reset Power Ups on start of new/reset difficulty, *can be called twice*
        if (GetCurrentLevelByDiff() == 0) GameManager.instance.GetPowerUpManagerByDiff().ResetPowerUpsSettings();
        // Reset collectedChests to 0 per level
        GameManager.instance.GetPowerUpManagerByDiff().ResetCurrentCollectedChests();
        PlayersSettingsManager.instance.ApplyPlayersSettings(); // Apply saved players data/settings
        UIManager.instance.Start(); // Reset/Update every relevant UI Elements
        // If player has won or is out of lives or difficulty is complete, don't generate a level
        // This is assuming the "Go" Button in DiffScene is interactable
        if (Player.hasWon || Player.isOutOfLives || isDifficultyComplete) {
            UIManager.instance.DimCanvasUI(); // Dim Canvas
            if (Player.hasWon) GameManager.instance.player.OnPlayerWon();
            else if (Player.isOutOfLives) GameManager.instance.player.OnPlayerOutOfLives();
            else OnLevelComplete(); // Handles this Difficulty complete scenario
            return; 
        }
        // Else, generate level
        StartLevelCoroutine(StartCoroutine(GenerateLevel()));
    }

    private void StartLevelCoroutine(Coroutine coroutine) {
        TryStopLevelCoroutine();
        this._levelCoroutine = coroutine;
    }

    private IEnumerator GenerateLevel() {
        UIManager.instance.DisplayLoadingText(); // Display "loading level" for fashion
        yield return new WaitForSeconds(this._timeBeforeLevelStart);

        // Reset all "Player" related settings before re-generating a level
        GameManager.instance.traitor.ResetLevelSettings();
        GameManager.instance.player.ResetLevelSettings();
        GridManager.instance.ClearAllTileTypes();
        
        UIManager.instance.DisplayLevelText(GetCurrentLevelByDiff() + 1); // Display current level
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty

        // Enables player and traitor after grid is generated
        GameManager.instance.EnablePlayers();

        // Set lineRenderer to the AI's spawn position
        GameManager.instance.traitor.SetLRPosCount(1); // Set LineRenderer position count to 1
        GameManager.instance.traitor.SetLineRendererStatus(true); // Enable LineRenderer
        GameManager.instance.traitor.SetLRPosition(0, PlayersManager.SpawnPosV3());

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
        if (!GameManager.instance.GetPowerUpManagerByDiff().hasTraitorsWake) GameManager.instance.traitor.SetLineRendererStatus(false);
        GameManager.instance.traitor.hasEnded = true; // Sets traitor.hasEnded to true after timeToMemorize is complete

        UIManager.instance.StartTimeToCompleteCoroutine();
        yield return new WaitUntil(() => !UIManager.instance.isCompleting);
    }

    public void DetermineNextEvent() {
        if (GameManager.instance.player.MovesEquals(GameManager.instance.traitor)) {
            // Calculate scores and display them
            ScoreManager.instance.CalculateScores(); UIManager.instance.UpdateScoreText();
            IncrementLevelsCompletedByDiff(); // Only increment if last level was completed
            // Increment totalLevelsCompleted if the current difficulty wasn't already completed
            if (!GetIsCurrentDiffCompleted()) ++this._totalLevelsCompleted;
            // If player beat the final level, call HandleLevelEnd -> WinScreen
            // Else if there's a next level, send player to the next level,
            // Else the player completed the difficulty: OnLevelComplete -> DiffCompleteScreen
            if (this._totalLevelsCompleted == GetTotalLevels()) {
                GameManager.instance.player.OnPlayerWon(); // Player won!
            } else {
                if (HasNextLevelForDifficulty()) {
                    // Save and apply if next level exist, instead let Player.OnDestroy do the job
                    PlayersSettingsManager.instance.SavePlayersSettings();
                    PlayersSettingsManager.instance.ApplyPlayersSettings(); // Apply saved players data/settings
                } else this.isDifficultyComplete = true; // Difficulty complete
                OnLevelComplete(); // calls HandleLevelEnd -> LevelComplete screen
            }
        } else GameManager.instance.player.OnPlayerLost(); // Player lost
    }
    
    private bool HasNextLevelForDifficulty() {
        switch(GameManager.instance.difficulty){
            case GameManager.Difficulty.Easy: {
                if (NextLevelExistsByDiff(GameManager.Difficulty.Easy)) return true;
                this.isEasyCompleted = true;
                return false;
            }
            case GameManager.Difficulty.Medium: {
                if (NextLevelExistsByDiff(GameManager.Difficulty.Medium)) return true;
                this.isMediumCompleted = true;
                return false; 
            }
            case GameManager.Difficulty.Hard: {
                if (NextLevelExistsByDiff(GameManager.Difficulty.Hard)) return true;
                this.isHardCompleted = true;
                return false;
            }
            default: return false;
        }  
    }

    public bool NextLevelExistsByDiff(GameManager.Difficulty diff) {
        return diff switch {
            GameManager.Difficulty.Easy => this.currentEasyLevelsCompleted + 1 <= this._maxEasyLevels,
            GameManager.Difficulty.Medium => this.currentMediumLevelsCompleted + 1 <= this._maxMediumLevels,
            GameManager.Difficulty.Hard => this.currentHardLevelsCompleted + 1 <= this._maxHardLevels,
            _ => false
        };
    }

    private void OnLevelComplete() {
        // Sfx is played in HandleLevelCompleteScreen -> Checks if Difficulty is complete
        this.isLevelComplete = true; HandleLevelEnd();
    }

    public void HandleLevelEnd() {
        StopAllCoroutines(true);
        UIManager.instance.DisplayEndScreen();
    }

    public void StopAllCoroutines(bool includeLevelCoroutine) {
        GameManager.instance.traitor.TryStopMoveSequenceCoroutine();
        UIManager.instance.TryStopTimeToMemorizeCoroutine();
        UIManager.instance.TryStopTimeToCompleteCoroutine();
        if (includeLevelCoroutine) TryStopLevelCoroutine();
    }
    
    private void TryStopLevelCoroutine() {
        if (this._levelCoroutine == null) return;
        StopCoroutine(this._levelCoroutine);
        this._levelCoroutine = null;
    }

    public int GetCurrentLevelByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this.currentEasyLevelsCompleted,
            GameManager.Difficulty.Medium => this.currentMediumLevelsCompleted,
            GameManager.Difficulty.Hard => this.currentHardLevelsCompleted,
            _ => 0
        };
    }
    
    public void ResetCurrentLevelByDiff(GameManager.Difficulty diff) {
        GameManager.instance.SetDifficulty(diff);
        // Reset current score to 0 only-if current score was greater than 0 by diff
        if (ScoreManager.instance.GetCurrentScoreByDiff() > 0) ScoreManager.instance.SetCurrentScoreByDiff(0f); 
        hasResetRun = true; // calls ResetRunState on GO
        switch (diff) {
            case GameManager.Difficulty.Easy: {
                if (!this.isEasyCompleted) this._totalLevelsCompleted -= this.currentEasyLevelsCompleted;
                this.currentEasyLevelsCompleted = 0; break;
            }
            case GameManager.Difficulty.Medium: {
                if (!this.isMediumCompleted) this._totalLevelsCompleted -= this.currentMediumLevelsCompleted;
                this.currentMediumLevelsCompleted = 0; break;
            }
            case GameManager.Difficulty.Hard: {
                if (!this.isHardCompleted) this._totalLevelsCompleted -= this.currentHardLevelsCompleted;
                this.currentHardLevelsCompleted = 0; break;
            }
        } 
    }
    
    public void ResetAll() {
        Player.hasWon = false; hasResetRun = true; // calls ResetRunState on GO
        ScoreManager.instance.ResetCurrentScore(); // Sets easy/medium/hardCurrenScore to 0
        isDifficultyComplete = false;
        this._totalLevelsCompleted = 0;
        this.currentEasyLevelsCompleted = 0;
        this.isEasyCompleted = false;
        this.currentMediumLevelsCompleted = 0;
        this.isMediumCompleted = false;
        this.currentHardLevelsCompleted = 0;
        this.isHardCompleted = false;
        GameManager.instance.ResetResetCounts();
    }
    
    // this is only for when the traitor's ship/traitor dies...
    public void HandleTraitorDeathByDiff() {
        this._totalLevelsCompleted = GetMaxLevelsByDiff();
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: {
                this.currentEasyLevelsCompleted = GetMaxEasyLevels();
                this.isEasyCompleted = true;
                HandleLevelEnd(); // calls DisplayEndScreen -> OnTraitorShipDestroyed
                break;
            }
            case GameManager.Difficulty.Medium: {
                this.currentMediumLevelsCompleted = GetMaxMediumLevels();
                this.isMediumCompleted = true;
                HandleLevelEnd(); // calls DisplayEndScreen -> OnTraitorShipDestroyed
                break;
            }
            case GameManager.Difficulty.Hard: {
                this.currentHardLevelsCompleted = GetMaxHardLevels();
                this.isHardCompleted = true;
                // Player wins if traitor is killed in final stage (Hard)
                GameManager.instance.player.OnPlayerWon(); // calls DisplayEndScreen -> Win Screen
                break; 
            }
        }
    }
    
    private void IncrementLevelsCompletedByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: ++this.currentEasyLevelsCompleted; break;
            case GameManager.Difficulty.Medium: ++this.currentMediumLevelsCompleted; break;
            case GameManager.Difficulty.Hard: ++this.currentHardLevelsCompleted; break;
        }
    }
    public int GetTotalLevelsCompleted() => this._totalLevelsCompleted;

    private bool GetIsCurrentDiffCompleted() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => this.isEasyCompleted,
            GameManager.Difficulty.Medium => this.isMediumCompleted,
            GameManager.Difficulty.Hard => this.isHardCompleted,
            _ => false
        };
    }
    
    private int GetMaxLevelsByDiff() {
        return GameManager.instance.difficulty switch {
            GameManager.Difficulty.Easy => GetMaxEasyLevels(),
            GameManager.Difficulty.Medium => GetMaxMediumLevels(),
            GameManager.Difficulty.Hard => GetMaxHardLevels(),
            _ => 0
        };
    }
    public int GetMaxEasyLevels() => this._maxEasyLevels;
    public int GetMaxMediumLevels() => this._maxMediumLevels;
    public int GetMaxHardLevels() => this._maxHardLevels;
    public int GetTotalLevels() => this._maxEasyLevels + this._maxMediumLevels + this._maxHardLevels;
}