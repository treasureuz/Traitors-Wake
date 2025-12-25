using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private float _timeBeforeLevelStart = 3f;
    
    public static LevelManager instance;

    private Coroutine _levelCoroutine;
    private int _currentLevel;
    private int _levelDiff;
    public static bool isGameEnded { get; private set; }
    
    void Awake() {
        instance = this;
        isGameEnded = true;
    }
    
    public IEnumerator GenerateLevel() {
        if (isGameEnded) {
            this._currentLevel = 0;
            this._levelDiff = 0;
            GameManager.instance.player.ResetPlayerSettings();
            GameManager.instance.traitor.ResetPlayerSettings();
            GameManager.instance.SetDifficulty(GameManager.instance.StartDifficulty);
        } 
        this._currentLevel++; // Increment current level
        this._levelDiff++; // Increment level per difficulty
        
        // Increment difficulty if current level is greater than levelEndPerDiff
        // Else continue with same difficulty from previous level
        if (this._levelDiff > GameManager.instance.levelsPerDiff) {
            this._levelDiff = 0; // Reset level per difficulty
            GameManager.instance.IncrementDifficulty();
        } else GameManager.instance.SetDifficulty(GameManager.instance.difficulty);
        
        // Set buttons to false and wait X amount of time before generating level
        UIManager.instance.SetActionButtons(false);
        yield return new WaitForSeconds(this._timeBeforeLevelStart);
        isGameEnded = false;
        
        // Reset all "Player" related settings before re-generating a level
        GameManager.instance.traitor.ResetLevelSettings();
        GameManager.instance.traitor.SetLRPosCount(0); // Reset LineRenderer position count
        GameManager.instance.player.ResetLevelSettings();
        GridManager.instance.ClearAllTiles();
        
        GameManager.instance.traitor.SetLRPosCount(1); // Set LineRenderer position count to 1
        GameManager.instance.traitor.SetLineRendererStatus(true); // Enable LineRenderer
        
        UIManager.instance.DisplayLevelText(this._currentLevel); // Display current level
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty
            
        // Set lineRenderer to the AI's spawn position
        GameManager.instance.traitor.SetLRPosition(0, PlayerManager.SpawnPosV3());
        
        // After move sequence, remove AI path trace, and enable player button actions
        yield return StartCoroutine(GameManager.instance.traitor.MoveSequence());
        yield return StartCoroutine(UIManager.instance.DisplayTimeToMemorize(GameManager.instance.timeToMemorize));

        // After timeToMemorize is done, wait an additional 0.5 seconds
        yield return new WaitForSeconds(0.5f); 
        
        UIManager.instance.SetActionButtons(true);
        GameManager.instance.traitor.SetLineRendererStatus(false); // Disable LineRenderer
        GameManager.instance.traitor.hasEnded = true;
            
        yield return StartCoroutine(UIManager.instance.DisplayTimeToComplete());
        UIManager.instance.SetActionButtons(false); // Disable buttons so player position isn't overwritten
        
        // After level completed, determine the next event
        DetermineNextEvent();
    }

    private void DetermineNextEvent() {
        if (GameManager.instance.player.MovesEquals(GameManager.instance.traitor)){
            // If the player beat the final level, then call EndGame -> Win Screen
            // Else, send the player to the next level
            if (this._currentLevel == GameManager.instance.GetTotalLevels()) {
                Player.hasWon = true;
                EndGame();
            } else {
                UIManager.instance.DisplayLoadingText();
                this._levelCoroutine = StartCoroutine(GenerateLevel());
            }
        } else {
            Player.hasWon = false;
            EndGame(); // -> Lose Screen
        }
    }

    public void EndGame() {
        if (this._levelCoroutine == null) return;
        StopAllCoroutines();
        this._levelCoroutine = null;
        isGameEnded = true;
        UIManager.instance.DisplayEndScreen();
    }
    public void SetLevelCoroutine(Coroutine coroutine) => this._levelCoroutine = coroutine;
}
