using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    [Header("Levels Per Difficulty")] 
    [SerializeField] public int _maxEasyLevels = 4;
    [SerializeField] private int _maxMediumLevels = 3;
    [SerializeField] private int _maxHardLevels = 2;
    
    [Header("Other settings")]
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
        ScoreManager.instance.SetCurrentScoreByDiff(0f); // Reset current score
        UIManager.instance.UpdateScoreText(false); // doesn't calculate score
        GameManager.instance.player.ResetPlayerSettings();
        GameManager.instance.traitor.ResetPlayerSettings();
        PlayersDataManager.instance.SavePlayersData();
        PowerUpManager.instance.ResetPowerUpsSettings(); // calls UIManager.DisablePowerUpSprites()
    }
    
    public void TryStartLevel() {
        StartLevelCoroutine(StartCoroutine(GenerateLevel()));
    }

    private void StartLevelCoroutine(Coroutine coroutine) {
        if (this._levelCoroutine != null) StopCoroutine(this._levelCoroutine);
        this._levelCoroutine = coroutine;
    }

    private IEnumerator GenerateLevel() {
        if (hasResetRun) ResetRunState(); // only works on start
        PlayersDataManager.instance.ApplyPlayersData(); // Apply saved players data/settings
        
        // Disable relevant UI elements 
        UIManager.instance.SetActionButtons(false);
        UIManager.instance.SetPauseButton(false);
        UIManager.instance.SetOnPauseButtons(false); // Disable Resume, Restart, Home on start
        UIManager.instance.SetEndScreenButtons(false);
        
        UIManager.instance.DisplayLoadingText(); // Display "loading level" for fashion
        yield return new WaitForSeconds(this._timeBeforeLevelStart);

        // Reset all "Player" related settings before re-generating a level
        GameManager.instance.traitor.ResetLevelSettings();
        GameManager.instance.player.ResetLevelSettings();
        GridManager.instance.ClearAllTileTypes();
        
        UIManager.instance.DisplayLevelText(this._totalLevelsCompleted + 1); // Display current level
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
        if (!PowerUpManager.instance.isLineTrace) GameManager.instance.traitor.SetLineRendererStatus(false);
        GameManager.instance.traitor.hasEnded = true; // Sets traitor.hasEnded to true after timeToMemorize is complete

        UIManager.instance.StartTimeToCompleteCoroutine();
        yield return new WaitUntil(() => !UIManager.instance.isCompleting);
    }

    public void DetermineNextEvent() {
        if (GameManager.instance.player.MovesEquals(GameManager.instance.traitor)) {
            // If the player beat the final level, then call EndGame -> Win Screen
            // Else, send the player to the next level
            if (this._totalLevelsCompleted == GetTotalLevels()) {
                Player.hasWon = true;
                HandleGameEnd(); // -> Win Screen
            } else {
                IncrementLevelsByDiff(); // Only increment if last level was completed
                // Set current collected chests to 0 for every level
                PowerUpManager.instance.ResetCurrentCollectedChests(); 
                // Start if player has completed all the levels for a certain difficulty
                // Else, take player back to DifficultySelectScene
                if (HasNextLevelForDifficulty()) {
                    // Save and apply if next level exist, instead let Player.OnDestroy do the job
                    PlayersDataManager.instance.SavePlayersData();
                    UIManager.instance.UpdateScoreText(true); // Calculates score
                    TryStartLevel(); // Start next level
                } else SceneManager.LoadScene("DifficultySelectScene");
            }
        } else {
            Player.hasWon = false;
            HandleGameEnd(); // -> Lose Screen
        }
    }
    
    private bool HasNextLevelForDifficulty() {
        switch(GameManager.instance.difficulty){
            case GameManager.Difficulty.Easy: {
                // Return true -- can advance, therefore difficulty incomplete
                if (this.currentEasyLevelsCompleted + 1 <= this._maxEasyLevels) {
                    if (!this.isCurrentEasyCompleted) ++this._totalLevelsCompleted;
                    return true;
                }
                // Return false -- can't advance, therefore difficulty completed
                this.isCurrentEasyCompleted = true;
                return false;
            }
            case GameManager.Difficulty.Medium: {
                // Return true -- can advance, therefore difficulty incomplete
                if (this.currentMediumLevelsCompleted + 1 <= this._maxMediumLevels) {
                    if (!this.isCurrentMediumCompleted) ++this._totalLevelsCompleted;
                    return true;
                }
                // Return false -- can't advance, therefore difficulty completed
                this.isCurrentMediumCompleted = true; 
                return false; 
            }
            case GameManager.Difficulty.Hard: {
                // Return true -- can advance, therefore difficulty incomplete
                if (this.currentHardLevelsCompleted + 1 <= this._maxHardLevels) {
                    if (!this.isCurrentHardCompleted) ++this._totalLevelsCompleted;
                    return true;
                }
                // Return false -- can't advance, therefore difficulty completed
                this.isCurrentHardCompleted = true;
                return false;
            }
            default: return false;
        }  
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

    private void IncrementLevelsByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this.currentEasyLevelsCompleted++; break;
            case GameManager.Difficulty.Medium: this.currentMediumLevelsCompleted++; break;
            case GameManager.Difficulty.Hard: this.currentHardLevelsCompleted++; break;
        }
    }

    public int GetMaxEasyLevels() => this._maxEasyLevels;
    public int GetMaxMediumLevels() => this._maxMediumLevels;
    public int GetMaxHardLevels() => this._maxHardLevels;
    
    public void ResetCurrentLevelByDiff() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: {
                this._totalLevelsCompleted -= this.currentEasyLevelsCompleted;
                this.currentEasyLevelsCompleted = 0; break;
            }
            case GameManager.Difficulty.Medium: {
                this._totalLevelsCompleted -= this.currentMediumLevelsCompleted;
                this.currentMediumLevelsCompleted = 0; break;
            }
            case GameManager.Difficulty.Hard: {
                this._totalLevelsCompleted -= this.currentHardLevelsCompleted;
                this.currentHardLevelsCompleted = 0; break;
            }
        }
    }

    public void ResetAllLevels() {
        this._totalLevelsCompleted = 0;
        this.currentEasyLevelsCompleted = 0;
        this.isCurrentEasyCompleted = false;
        this.currentMediumLevelsCompleted = 0;
        this.isCurrentMediumCompleted = false;
        this.currentHardLevelsCompleted = 0;
        this.isCurrentHardCompleted = false;
    }
    
    public int GetTotalLevelsCompleted() => this._totalLevelsCompleted;
    private int GetTotalLevels() {
        return this._maxEasyLevels + this._maxMediumLevels + this._maxHardLevels;
    }
}
