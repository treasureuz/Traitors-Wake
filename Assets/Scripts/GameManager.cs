using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Player _aiPlayerPrefab;

    [SerializeField] private Vector2Int _finishPos;
    [SerializeField] private Difficulty _startDifficulty = Difficulty.Easy;
    
    [Header("Number of Directions")]
    [SerializeField] private int _easyNumOfDirs = 5;
    [SerializeField] private int _normalNumOfDirs = 3;
    [SerializeField] private int _hardNumOfDirs = 2;
    
    [Header("Min/Max Grid Numbers")]
    [SerializeField] private int _easyMinGrid = 4;
    [SerializeField] private int _easyMaxGrid = 8;

    [SerializeField] private int _normalMinGrid = 8;
    [SerializeField] private int _normalMaxGrid = 12;

    [SerializeField] private int _hardMinGrid = 12;
    [SerializeField] private int _hardMaxGrid = 17;

    [Header("Level End Per Difficulty")] 
    [SerializeField] private int _easyLevelEnd = 4;
    [SerializeField] private int _normalLevelEnd = 7;
    [SerializeField] private int _hardLevelEnd = 9;
    
    [Header("Time To Memorize")]
    [SerializeField] private float _easyTimeToMemorize = 3.5f;
    [SerializeField] private float _normalTimeToMemorize = 3f;
    [SerializeField] private float _hardTimeToMemorize = 2.3f;
    
    [Header("Time To Complete")]
    [SerializeField] private float _easyTimeToComplete = 12.8f;
    [SerializeField] private float _normalTimeToComplete = 12.5f;
    [SerializeField] private float _hardTimeToComplete = 11.5f;
    
    [Header("Time To Move")]
    [SerializeField] private float _easyAITimeToMove = 0.32f;
    [SerializeField] private float _normalAITimeToMove = 0.18f;
    [SerializeField] private float _hardAITimeToMove = 0.1f;

    public Vector2Int FinishPos => this._finishPos;
    public Difficulty StartDifficulty => this._startDifficulty;
    
    public static GameManager instance;
        
    public Player player { get; private set; }
    public Player aiPlayer { get; private set; }
    public AIManager aiManager { get; private set; }
    
    public int numOfDirs {get; private set;}
    public int levelEndPerDiff { get; private set; }
    public float timeToMemorize { get; private set; }
    public float timeToComplete { get; private set; }

    public enum Difficulty {
        Easy,
        Normal,
        Hard
    }
    public Difficulty difficulty { get; private set; }

    void Awake() {
        instance = this;
        this.aiPlayer = Instantiate(this._aiPlayerPrefab, Player.SpawnPos(), Quaternion.identity);
        this.player = Instantiate(this._playerPrefab, Player.SpawnPos(), Quaternion.identity);
        this.aiManager = this.aiPlayer.GetComponent<AIManager>();
    }
    
    void Start() {
        StartCoroutine(LevelManager.instance.GenerateLevel());
    }
    
    private void HandleDifficultySettings() {
        switch (this.difficulty) {
            case Difficulty.Easy: {
                this.numOfDirs = this._easyNumOfDirs; 
                GridManager.instance.SetWidth(Random.Range(this._easyMinGrid, this._easyMaxGrid));
                GridManager.instance.SetHeight(Random.Range(this._easyMinGrid, this._easyMaxGrid));
                this.levelEndPerDiff = this._easyLevelEnd;
                this.timeToMemorize = this._easyTimeToMemorize;
                this.timeToComplete = this._easyTimeToComplete;
                this.aiManager.aiTimeToMove = this._easyAITimeToMove;
                break;
            }
            case Difficulty.Normal: {
                this.numOfDirs = this._normalNumOfDirs; 
                GridManager.instance.SetWidth(Random.Range(this._normalMinGrid, this._normalMaxGrid));
                GridManager.instance.SetHeight(Random.Range(this._normalMinGrid, this._normalMaxGrid));
                this.levelEndPerDiff = this._normalLevelEnd;
                this.timeToMemorize = this._normalTimeToMemorize;
                this.timeToComplete = this._normalTimeToComplete;
                this.aiManager.aiTimeToMove = this._normalAITimeToMove;
                break;
            }
            case Difficulty.Hard: {
                this.numOfDirs = this._hardNumOfDirs; 
                GridManager.instance.SetWidth(Random.Range(this._hardMinGrid, this._hardMaxGrid));
                GridManager.instance.SetHeight(Random.Range(this._hardMinGrid, this._hardMaxGrid));
                this.levelEndPerDiff = this._hardLevelEnd;
                this.timeToMemorize = this._hardTimeToMemorize;
                this.timeToComplete = this._hardTimeToComplete;
                this.aiManager.aiTimeToMove = this._hardAITimeToMove;
                break;
            }
        }
        this._finishPos = new Vector2Int(GridManager.instance.Width - 1, GridManager.instance.Height - 1);
    }
    
    public void IncrementDifficulty() {
        if (this.difficulty == Difficulty.Hard) return;
        SetDifficulty(++this.difficulty);
    }
    public void SetDifficulty(Difficulty diff) {
        this.difficulty = diff;
        HandleDifficultySettings();
    }
}
