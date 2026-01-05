using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Traitor _traitorPrefab;

    [SerializeField] private Vector2Int _finishPos;
    [SerializeField] private Difficulty _startDifficulty = Difficulty.Easy;
    
    [Header("Levels Per Difficulty")] 
    [SerializeField] private int _easyLevels = 4;
    [SerializeField] private int _normalLevels = 3;
    [SerializeField] private int _hardLevels = 2;
    
    [Header("Number of Up/Right Directions")]
    [SerializeField] private int _easyNumOfDirs = 5;
    [SerializeField] private int _normalNumOfDirs = 3;
    [SerializeField] private int _hardNumOfDirs = 2;
    
    [Header("Min/Max Grid Numbers")]
    [SerializeField] private int _easyWidth = 7;
    [SerializeField] private int _easyHeight = 5;
    [SerializeField] private int _normalWidth = 8;
    [SerializeField] private int _normalHeight = 6;
    [SerializeField] private int _hardWidth = 9;
    [SerializeField] private int _hardHeight = 7;
    
    [Header("Number of Chests/Obstacles")]
    [SerializeField] private int _easyNumOfChests = 3;
    [SerializeField] private int _easyNumOfObstacles = 4;
    [SerializeField] private int _normalNumOfChests = 4;
    [SerializeField] private int _normalNumOfObstacles = 5;
    [SerializeField] private int _hardNumOfChests = 5;
    [SerializeField] private int _hardNumOfObstacles = 6;
    
    [Header("Time To Memorize")]
    [SerializeField] private float _easyTimeToMemorize = 3.5f;
    [SerializeField] private float _normalTimeToMemorize = 3f;
    [SerializeField] private float _hardTimeToMemorize = 2.3f;
    
    [Header("Time To Complete")]
    [SerializeField] private float _easyTimeToComplete = 11.8f;
    [SerializeField] private float _normalTimeToComplete = 12.5f;
    [SerializeField] private float _hardTimeToComplete = 13.2f;
    
    [Header("Player Time To Move")]
    [SerializeField] private float _easyPlayerTimeToMove = 0.3f;
    [SerializeField] private float _normalPlayerTimeToMove = 0.28f;
    [SerializeField] private float _hardPlayerTimeToMove = 0.25f;
    
    [Header("Traitor Time To Move")]
    [SerializeField] private float _easyTraitorTimeToMove = 0.42f;
    [SerializeField] private float _normalTraitorTimeToMove = 0.35f;
    [SerializeField] private float _hardTraitorTimeToMove = 0.3f;

    [Header("Traitor Min/Max Time Between Shots")] 
    [SerializeField] private float _easyMinTimeBetweenShots = 3.5f;
    [SerializeField] private float _easyMaxTimeBetweenShots = 8f;
    [SerializeField] private float _normalMinTimeBetweenShots = 3f;
    [SerializeField] private float _normalMaxTimeBetweenShots = 7f;
    [SerializeField] private float _hardMinTimeBetweenShots = 2.5f;
    [SerializeField] private float _hardMaxTimeBetweenShots = 6f;
    
    public Vector2Int FinishPos => this._finishPos;
    public Difficulty StartDifficulty => this._startDifficulty;
    
    public static GameManager instance;
        
    public Player player { get; private set; }
    public Traitor traitor { get; private set; }
    public PWeaponManager pWeaponManager { get; private set; }
    public TWeaponManager tWeaponManager { get; private set; }
    
    public int numOfDirs {get; private set;}
    public int numOfChests { get; private set; }
    public int numOfObstacles { get; private set; }
    public int levelsPerDiff { get; private set; }
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
        this.player = Instantiate(this._playerPrefab, PlayerManager.SpawnPosV3(), Quaternion.identity);
        this.player.gameObject.SetActive(false);
        this.traitor = Instantiate(this._traitorPrefab, PlayerManager.SpawnPosV3(), Quaternion.identity);
        this.traitor.gameObject.SetActive(false);
        this.pWeaponManager = this.player.GetComponentInChildren<PWeaponManager>();
        this.tWeaponManager = this.traitor.GetComponentInChildren<TWeaponManager>();
    }
    
    void Start() {
        LevelManager.instance.SetLevelCoroutine(StartCoroutine(LevelManager.instance.GenerateLevel()));
    }
    
    private void HandleDifficultySettings() {
        switch (this.difficulty) {
            case Difficulty.Easy: {
                this.numOfDirs = this._easyNumOfDirs; 
                this.numOfChests = this._easyNumOfChests;
                this.numOfObstacles = this._easyNumOfObstacles;
                GridManager.instance.SetWidth(this._easyWidth);
                GridManager.instance.SetHeight(this._easyHeight);
                this.levelsPerDiff = this._easyLevels;
                this.timeToMemorize = this._easyTimeToMemorize;
                this.timeToComplete = this._easyTimeToComplete;
                this.player.SetTimeToMove(this._easyPlayerTimeToMove);
                this.traitor.SetTimeToMove(this._easyTraitorTimeToMove);
                this.tWeaponManager.SetMinTimeBetweenShots(this._easyMinTimeBetweenShots);
                this.tWeaponManager.SetMaxTimeBetweenShots(this._easyMaxTimeBetweenShots);
                break;
            }
            case Difficulty.Normal: {
                this.numOfDirs = this._normalNumOfDirs; 
                this.numOfChests = this._normalNumOfChests;
                this.numOfObstacles = this._normalNumOfObstacles;
                GridManager.instance.SetWidth(this._normalWidth);
                GridManager.instance.SetHeight(this._normalHeight);
                this.levelsPerDiff = this._normalLevels;
                this.timeToMemorize = this._normalTimeToMemorize;
                this.timeToComplete = this._normalTimeToComplete;
                this.player.SetTimeToMove(this._normalPlayerTimeToMove);
                this.traitor.SetTimeToMove(this._normalTraitorTimeToMove);
                this.tWeaponManager.SetMinTimeBetweenShots(this._normalMinTimeBetweenShots);
                this.tWeaponManager.SetMaxTimeBetweenShots(this._normalMaxTimeBetweenShots);
                break;
            }
            case Difficulty.Hard: {
                this.numOfDirs = this._hardNumOfDirs; 
                this.numOfChests = this._hardNumOfChests;
                this.numOfObstacles = this._hardNumOfObstacles;
                GridManager.instance.SetWidth(this._hardWidth);
                GridManager.instance.SetHeight(this._hardHeight);
                this.levelsPerDiff = this._hardLevels;
                this.timeToMemorize = this._hardTimeToMemorize;
                this.timeToComplete = this._hardTimeToComplete;
                this.player.SetTimeToMove(this._hardPlayerTimeToMove);
                this.traitor.SetTimeToMove(this._hardTraitorTimeToMove);
                this.tWeaponManager.SetMinTimeBetweenShots(this._hardMinTimeBetweenShots);
                this.tWeaponManager.SetMaxTimeBetweenShots(this._hardMaxTimeBetweenShots);
                break;
            }
        }
        // Set finishPos based on the above set width and height
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

    public void SetTimeToComplete(float time) {
        this.timeToComplete = time;
    }
    
    public void SetTimeToMemorize(float time) {
        this.timeToMemorize = time;
    }
    
    public int GetTotalLevels() {
        return this._easyLevels + this._normalLevels + this._hardLevels;
    }
}
