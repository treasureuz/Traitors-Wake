using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private float _timeBeforeLevelStart = 3f;
    
    public static LevelManager instance;
    private int _currentLevel;
    
    void Awake() {
        instance = this;
        Player.isReset = true;
    }
    
    public IEnumerator GenerateLevel() {
        if (Player.isReset) {
            this._currentLevel = 0;
            GameManager.instance.SetDifficulty(GameManager.instance.StartDifficulty);
            Player.isReset = false;
        } 
        this._currentLevel++; // Increment current level

        // Increment difficulty if current level is greater than levelEndPerDiff
        // Else continue with same difficulty from previous level
        if (this._currentLevel > GameManager.instance.levelEndPerDiff) {
            GameManager.instance.IncrementDifficulty();
        } else GameManager.instance.SetDifficulty(GameManager.instance.difficulty);
        
        // Set buttons to false and wait X amount of time before generating level
        UIManager.instance.SetButtons(false);
        yield return new WaitForSeconds(this._timeBeforeLevelStart);
            
        // Reset all "Player" settings before re-generating a level
        GameManager.instance.aiPlayer.ResetSettings();
        GameManager.instance.player.ResetSettings();
        GridManager.instance.ClearAllTiles();
            
        GameManager.instance.aiManager.SetLRPosCount(1); // Set LineRenderer position count to 1
            
        UIManager.instance.DisplayLevelText(this._currentLevel); // Display current level
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty
            
        // Set lineRenderer to the AI's spawn position
        GameManager.instance.aiManager.SetLRPosition(0, Player.SpawnPos());
            
        // After move sequence, remove AI path trace, and enable player button actions
        yield return StartCoroutine(GameManager.instance.aiManager.MoveSequence());
        yield return StartCoroutine(UIManager.instance.DisplayTimeToMemorize(GameManager.instance.timeToMemorize));
            
        // After timeToMemorize is done, wait an additional 0.5 seconds and sets Player.isMemorizing to false so player can move
        yield return new WaitForSeconds(0.5f); 
        // Add powerup check
        GameManager.instance.aiManager.SetLRPosCount(0); // Reset LineRenderer position count
        Player.isMemorizing = false;
        UIManager.instance.SetButtons(true);
            
        yield return StartCoroutine(UIManager.instance.DisplayTimeToComplete(GameManager.instance.timeToComplete));
        UIManager.instance.SetButtons(false); // Disable buttons so player position isn't overwritten
            
        // Compare stack of moves between the AI and the player
        // Stops the loop if you failed the level
        if (!UIManager.instance.CanAdvanceLevel()) yield break;
        
        //Recursively generate new randomized levels
        StartCoroutine(GenerateLevel());
    }
}
