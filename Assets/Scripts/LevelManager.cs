using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private float _timeBeforeLevelStart = 1f;
    
    public static LevelManager instance;

    private Coroutine _levelCoroutine;
    private int _currentLevel;
    private int _levelDiff;
    public static bool isGameEnded;

    void Awake() {
        instance = this;
        isGameEnded = true;
    }

    public IEnumerator GenerateLevel() {
        if (isGameEnded) {
            ScoreManager.instance.SetCurrentScore(0f); UIManager.instance.UpdateScoreText();
            isGameEnded = false; GameManager.isPaused = false;
            this._currentLevel = 0; this._levelDiff = 0;
            GameManager.instance.player.ResetPlayerSettings();
            GameManager.instance.traitor.ResetPlayerSettings();
            PowerUpManager.instance.ResetPowerUps(); // calls UIManager.DisablePowerUpSprites()
            GameManager.instance.SetDifficulty(GameManager.instance.StartDifficulty);
        }
        this._currentLevel++; // Increment current level
        this._levelDiff++; // Increment level per difficulty

        // Increment difficulty if current level is greater than levelEndPerDiff
        // Else continue with same difficulty from previous level
        if (this._levelDiff > GameManager.instance.levelsPerDiff) {
            this._levelDiff = 1; // Reset levelDiff per difficulty to 1
            PowerUpManager.instance.ResetPowerUps(); // Reset power up stats to 0 and disables hotbar sprites
            GameManager.instance.IncrementDifficulty();
        } else GameManager.instance.SetDifficulty(GameManager.instance.difficulty);

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
        
        UIManager.instance.DisplayLevelText(this._currentLevel); // Display current level
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty

        // Enables player and traitor after grid is generated
        GameManager.instance.player.gameObject.SetActive(true);
        GameManager.instance.traitor.gameObject.SetActive(true);
            
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
        if (!PowerUpManager.instance.isLineTrace) {
            GameManager.instance.traitor.SetLineRendererStatus(false);
        }
        GameManager.instance.traitor.hasEnded = true; // Sets traitor.hasEnded to true after timeToMemorize is complete
            
        UIManager.instance.StartTimeToCompleteCoroutine();
        yield return new WaitUntil(() => !UIManager.instance.isCompleting);
        UIManager.instance.SetActionButtons(false); // Disable buttons so player position isn't overwritten
        
        // After level completed, disable UI elements and determine the next event
        UIManager.instance.SetPauseButton(false); DetermineNextEvent();
    }

    private void DetermineNextEvent() {
        if (GameManager.instance.player.MovesEquals(GameManager.instance.traitor)){
            // If the player beat the final level, then call EndGame -> Win Screen
            // Else, send the player to the next level
            if (this._currentLevel == GameManager.instance.GetTotalLevels()) {
                Player.hasWon = true;
                EndGame(); // -> Win Screen
            } else {
                // Continue levels
                UIManager.instance.UpdateScoreText();
                UIManager.instance.DisplayLoadingText();
                this._levelCoroutine = StartCoroutine(GenerateLevel());
            }
        } else {
            Player.hasWon = false;
            isGameEnded = true;
            EndGame(); // -> Lose Screen
        }
    }

    public void EndGame() {
        if (this._levelCoroutine == null) return;
        StopAllCoroutines();
        UIManager.instance.DisplayEndScreen();
    }

    private new void StopAllCoroutines() {
        GameManager.instance.traitor.StopMoveSequenceCoroutine();
        UIManager.instance.StopTimeToMemorizeCoroutine();
        UIManager.instance.StopTimeToCompleteCoroutine();
        StopCoroutine(this._levelCoroutine); this._levelCoroutine = null;
        isGameEnded = true;
    }
    
    public void StartLevelCoroutine(Coroutine coroutine) {
        if (this._levelCoroutine != null) StopCoroutine(this._levelCoroutine);
        this._levelCoroutine = coroutine;
    }

    public void ResetLevelDifficulty() => this._levelDiff = 1;
}
