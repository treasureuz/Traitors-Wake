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
    
    public static LevelManager instance;
    private Coroutine _levelCoroutine;

    public static bool hasResetRun;
    
    private int _totalLevelsCompleted;
    public int currentEasyLevelsCompleted { get; private set; }
    public int currentMediumLevelsCompleted { get; private set; }
    public int currentHardLevelsCompleted { get; private set; }

    public bool isCurrentEasyCompleted { get; private set; }
    public bool isCurrentMediumCompleted { get; private set; }
    public bool isCurrentHardCompleted { get; private set; }

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
        ScoreManager.instance.SetCurrentScoreByDiff(0f); // Reset current score by diff
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
        if (hasResetRun) ResetRunState(); // only works on start
        // Reset Power Ups on start of new/reset difficulty, *can be called twice*
        if (GetCurrentLevelByDiff() == 0) GameManager.instance.GetPowerUpManagerByDiff().ResetPowerUpsSettings();
        UIManager.instance.Start(); // Reset relevant UI Elements
        // If player has won or lost, don't generate a level
        if (Player.hasWon || Player.isOutOfLives) {
            UIManager.instance.DimCanvasUI(); // Dim Canvas
            if (Player.hasWon) GameManager.instance.player.OnPlayerWon();
            else GameManager.instance.player.OnPlayerOutOfLives();
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
        if (!GameManager.instance.GetPowerUpManagerByDiff().isLineTrace) GameManager.instance.traitor.SetLineRendererStatus(false);
        GameManager.instance.traitor.hasEnded = true; // Sets traitor.hasEnded to true after timeToMemorize is complete

        UIManager.instance.StartTimeToCompleteCoroutine();
        yield return new WaitUntil(() => !UIManager.instance.isCompleting);
    }

    public void DetermineNextEvent() {
        if (GameManager.instance.player.MovesEquals(GameManager.instance.traitor)) {
            // Calculate scores and display them
            ScoreManager.instance.CalculateScores();
            UIManager.instance.UpdateScoreText();
            IncrementLevelsCompletedByDiff(); // Only increment if last level was completed
            // Set currentCollectedChests to 0 per level
            GameManager.instance.GetPowerUpManagerByDiff().ResetCurrentCollectedChests(); 
            // If next level exists, send the player to the next level
            // Else if player beat the final level and hasn't already won, call EndGame -> Win Screen
            // Else, player completed a difficulty so take them back to DifficultySelectScene
            if (HasNextLevelForDifficulty()) {
                // Save and apply if next level exist, instead let Player.OnDestroy do the job
                PlayersSettingsManager.instance.SavePlayersSettings();
                PlayersSettingsManager.instance.ApplyPlayersSettings(); // Apply saved players data/settings7
                TryStartLevel(); // Start next level
            } else if (!Player.hasWon && this._totalLevelsCompleted == GetTotalLevels()) {
                GameManager.instance.player.OnPlayerWon(); // Player won!
            } else SceneManager.LoadScene("DifficultySelectScene"); // Difficulty complete
        } else GameManager.instance.player.OnPlayerLost(); // Player lost
    }
    
    private bool HasNextLevelForDifficulty() {
        switch(GameManager.instance.difficulty){
            case GameManager.Difficulty.Easy: {
                // Return true -- can advance, therefore difficulty incomplete
                if (!this.isCurrentEasyCompleted) ++this._totalLevelsCompleted;
                if (NextLevelExistsByDiff(GameManager.Difficulty.Easy)) return true;
                // Return false -- can't advance, therefore difficulty completed
                this.isCurrentEasyCompleted = true;
                return false;
            }
            case GameManager.Difficulty.Medium: {
                // Return true -- can advance, therefore difficulty incomplete
                if (!this.isCurrentMediumCompleted) ++this._totalLevelsCompleted;
                if (NextLevelExistsByDiff(GameManager.Difficulty.Medium)) return true;
                // Return false -- can't advance, therefore difficulty completed
                this.isCurrentMediumCompleted = true; 
                return false; 
            }
            case GameManager.Difficulty.Hard: {
                // Return true -- can advance, therefore difficulty incomplete
                if (!this.isCurrentHardCompleted) ++this._totalLevelsCompleted;
                if (NextLevelExistsByDiff(GameManager.Difficulty.Hard)) return true;
                // Return false -- can't advance, therefore difficulty completed
                this.isCurrentHardCompleted = true;
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
    
    public void HandleGameEnd() {
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
        hasResetRun = true; // calls ResetRunState on GO
        switch (diff) {
            case GameManager.Difficulty.Easy: {
                if (!this.isCurrentEasyCompleted) this._totalLevelsCompleted -= this.currentEasyLevelsCompleted;
                this.currentEasyLevelsCompleted = 0; break;
            }
            case GameManager.Difficulty.Medium: {
                if (!this.isCurrentMediumCompleted) this._totalLevelsCompleted -= this.currentMediumLevelsCompleted;
                this.currentMediumLevelsCompleted = 0; break;
            }
            case GameManager.Difficulty.Hard: {
                if (!this.isCurrentHardCompleted) this._totalLevelsCompleted -= this.currentHardLevelsCompleted;
                this.currentHardLevelsCompleted = 0; break;
            }
        } 
    }
    
    public void ResetAll() {
        Player.hasWon = false; hasResetRun = true; // calls ResetRunState on GO
        this._totalLevelsCompleted = 0;
        this.currentEasyLevelsCompleted = 0;
        this.isCurrentEasyCompleted = false;
        this.currentMediumLevelsCompleted = 0;
        this.isCurrentMediumCompleted = false;
        this.currentHardLevelsCompleted = 0;
        this.isCurrentHardCompleted = false;
        GameManager.instance.ResetResetCounts();
    }
    
    // this is only for when the traitor dies...
    public void ActivateAllLevelsByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: {
                this.currentEasyLevelsCompleted = GetMaxEasyLevels();
                this.isCurrentEasyCompleted = true; break;
            }
            case GameManager.Difficulty.Medium: {
                this.currentMediumLevelsCompleted = GetMaxMediumLevels();
                this.isCurrentMediumCompleted = true; break;
            }
            case GameManager.Difficulty.Hard: {
                this.currentHardLevelsCompleted = GetMaxHardLevels();
                this.isCurrentHardCompleted = true; break;
            }
        }
        this._totalLevelsCompleted = GetMaxLevelsByDiff();
        // Take player back to DiffSelectScene with completed difficulty
        SceneManager.LoadScene("DifficultySelectScene"); 
    }
    
    private void IncrementLevelsCompletedByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: ++this.currentEasyLevelsCompleted; break;
            case GameManager.Difficulty.Medium: ++this.currentMediumLevelsCompleted; break;
            case GameManager.Difficulty.Hard: ++this.currentHardLevelsCompleted; break;
        }
    }
    public int GetTotalLevelsCompleted() => this._totalLevelsCompleted;
    
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
    public int GetTotalLevels() {
        return this._maxEasyLevels + this._maxMediumLevels + this._maxHardLevels;
    }
}
