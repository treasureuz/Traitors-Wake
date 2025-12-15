using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private float _timeBeforeLevelStart = 3f;
    
    public static LevelManager instance;
    
    private AIManager _aiManager;
    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.SetDifficulty(GameManager.Difficulty.Easy);
        this._aiManager = GameManager.instance._aiPlayer.GetComponent<AIManager>();
    }
    
    public IEnumerator StartLevel(AIManager aiManager) {
        // Gives time for resources to load
        yield return new WaitForSeconds(this._timeBeforeLevelStart);
        
        //Reset all "Player" settings before creating a level
        GameManager.instance._aiPlayer.ResetSettings();
        GameManager.instance._player.ResetSettings();
   
        GridManager.instance.GenerateGrid(); // Generates grid based on difficulty
        
        // After move sequence, remove AI path trace, and enable player button actions
        yield return StartCoroutine(aiManager.MoveSequence());
        
        // TODO: **Insert UIManager.instance.DisplayTimeToMemorize() here**
        
        yield return new WaitForSeconds(GameManager.instance.timeToMemorize);
        this._aiManager.ResetLineRenderer();
        UIManager.instance.SetButtons(true);
        
        // TODO: **Insert UIManager.instance.DisplayTimeToComplete() here**
        
        // Set player.isEnded to true after timeToComplete was done or the submit button was clicked
        var timer = 0f;
        while (!GameManager.instance._player.isEnded) {
            if (timer >= GameManager.instance.timeToComplete) break;
            timer += Time.deltaTime;
            yield return null;
        }
        GameManager.instance._player.isEnded = true; 
        
        UIManager.instance.SetButtons(false); // Disable buttons so player position isn't overriden
        
        // Compare stack of moves between the AI and the player;
        if (!GameManager.instance._aiPlayer.CompareMoves(GameManager.instance._player)) yield break;
        GameManager.instance.IncrementDifficulty();
        
        //Recursively create a new randomized level (based on difficulty)
        StartCoroutine(StartLevel(aiManager));
    }
}
