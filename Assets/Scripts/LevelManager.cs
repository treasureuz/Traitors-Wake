using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private float _timeBeforeLevelStart = 3f;
    
    public static LevelManager instance;
    private int _currentLevel;
    private int _levelDiff;
    
    void Awake() {
        instance = this;
        Player.hasResetLevel = true;
    }
    
    public IEnumerator GenerateLevel() {
        // Set buttons to false and wait X amount of time before generating level
        UIManager.instance.SetButtons(false);
        yield return new WaitForSeconds(this._timeBeforeLevelStart);
        if (Player.hasResetLevel) {
            this._currentLevel = 0;
            this._levelDiff = 0;
            GameManager.instance.SetDifficulty(GameManager.instance.StartDifficulty);
            Player.hasResetLevel = false;
        } 
        this._currentLevel++; // Increment current level
        this._levelDiff++; // Increment level per difficulty
        
        // Increment difficulty if current level is greater than levelEndPerDiff
        // Else continue with same difficulty from previous level
        if (this._levelDiff > GameManager.instance.levelsPerDiff) {
            this._levelDiff = 0; // Reset level per difficulty
            GameManager.instance.IncrementDifficulty();
        } else GameManager.instance.SetDifficulty(GameManager.instance.difficulty);
            
        // Reset all "Player" related settings before re-generating a level
        GameManager.instance.traitor.ResetSettings();
        GameManager.instance.traitorManager.SetLRPosCount(0); // Reset LineRenderer position count
        GameManager.instance.player.ResetSettings();
        GridManager.instance.ClearAllTiles();
        
        GameManager.instance.traitorManager.SetLRPosCount(1); // Set LineRenderer position count to 1
        GameManager.instance.traitorManager.SetLineRendererStatus(true); // Enable LineRenderer
        
        UIManager.instance.DisplayLevelText(this._currentLevel); // Display current level
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty
            
        // Set lineRenderer to the AI's spawn position
        GameManager.instance.traitorManager.SetLRPosition(0, Player.SpawnPosV3());
        
        // After move sequence, remove AI path trace, and enable player button actions
        yield return StartCoroutine(GameManager.instance.traitorManager.MoveSequence());
        yield return StartCoroutine(UIManager.instance.DisplayTimeToMemorize(GameManager.instance.timeToMemorize));

        // After timeToMemorize is done, wait an additional 0.5 seconds
        yield return new WaitForSeconds(0.5f); 
        
        GameManager.instance.traitorManager.SetLineRendererStatus(false); // Disable LineRenderer
        GameManager.instance.traitor.hasEnded = true;
        UIManager.instance.SetButtons(true);
            
        yield return StartCoroutine(UIManager.instance.DisplayTimeToComplete());
        UIManager.instance.SetButtons(false); // Disable buttons so player position isn't overwritten
            
        // Compare stack of moves between the AI and the player
        // Stops the loop if you failed the level and display text
        UIManager.instance.DisplayFinishText();
        if (!CanAdvanceLevel()) yield break;
        
        // Recursively generate new randomized levels
        StartCoroutine(GenerateLevel());
    }

    public static bool CanAdvanceLevel() {
        return GameManager.instance.player.MovesEquals(GameManager.instance.traitor);
    }
}
