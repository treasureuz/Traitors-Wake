using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Traitor _traitorPrefab;

    [SerializeField] private Vector2Int _finishPos;
    [SerializeField] private Difficulty _startDifficulty = Difficulty.Easy;
    
    [Header("Number of Directions")]
    [SerializeField] private int _easyNumOfDirs = 5;
    [SerializeField] private int _normalNumOfDirs = 3;
    [SerializeField] private int _hardNumOfDirs = 2;
    
    [Header("Min/Max Grid Numbers")]
    [SerializeField] private int _minWidth = 7;
    [SerializeField] private int _maxWidth = 9;
    [SerializeField] private int _minHeight = 6;
    [SerializeField] private int _maxHeight = 7;
    
    [Header("Levels Per Difficulty")] 
    [SerializeField] private int _easyLevels = 4;
    [SerializeField] private int _normalLevels = 3;
    [SerializeField] private int _hardLevels = 2;
    
    [Header("Time To Memorize")]
    [SerializeField] private float _easyTimeToMemorize = 3.5f;
    [SerializeField] private float _normalTimeToMemorize = 3f;
    [SerializeField] private float _hardTimeToMemorize = 2.3f;
    
    [Header("Time To Complete")]
    [SerializeField] private float _easyTimeToComplete = 12.8f;
    [SerializeField] private float _normalTimeToComplete = 12.5f;
    [SerializeField] private float _hardTimeToComplete = 11.5f;
    
    [Header("Time To Move")]
    [SerializeField] private float _easyTraitorTimeToMove = 0.42f;
    [SerializeField] private float _normalTraitorTimeToMove = 0.35f;
    [SerializeField] private float _hardTraitorTimeToMove = 0.3f;

    public Vector2Int FinishPos => this._finishPos;
    public Difficulty StartDifficulty => this._startDifficulty;
    
    public static GameManager instance;
        
    public Player player { get; private set; }
    public Traitor traitor { get; private set; }
    public PWeaponManager pWeaponManager { get; private set; }
    public TWeaponManager tWeaponManager { get; private set; }
    
    public int numOfDirs {get; private set;}
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
        this.traitor = Instantiate(this._traitorPrefab, PlayerManager.SpawnPosV3(), Quaternion.identity);
        this.player = Instantiate(this._playerPrefab, PlayerManager.SpawnPosV3(), Quaternion.identity);
        this.pWeaponManager = this.player.GetComponent<PWeaponManager>();
        this.tWeaponManager = this.traitor.GetComponent<TWeaponManager>();
    }
    
    void Start() {
        LevelManager.instance.SetLevelCoroutine(StartCoroutine(LevelManager.instance.GenerateLevel()));
    }
    
    private void HandleDifficultySettings() {
        switch (this.difficulty) {
            case Difficulty.Easy: {
                this.numOfDirs = this._easyNumOfDirs; 
                this.levelsPerDiff = this._easyLevels;
                this.timeToMemorize = this._easyTimeToMemorize;
                this.timeToComplete = this._easyTimeToComplete;
                this.traitor.SetTimeToMove(this._easyTraitorTimeToMove);
                break;
            }
            case Difficulty.Normal: {
                this.numOfDirs = this._normalNumOfDirs; 
                this.levelsPerDiff = this._normalLevels;
                this.timeToMemorize = this._normalTimeToMemorize;
                this.timeToComplete = this._normalTimeToComplete;
                this.traitor.SetTimeToMove(this._normalTraitorTimeToMove);
                break;
            }
            case Difficulty.Hard: {
                this.numOfDirs = this._hardNumOfDirs; 
                this.levelsPerDiff = this._hardLevels;
                this.timeToMemorize = this._hardTimeToMemorize;
                this.timeToComplete = this._hardTimeToComplete;
                this.traitor.SetTimeToMove(this._hardTraitorTimeToMove);
                break;
            }
        }
        // Randomize width and height (not based on difficulty) 
        GridManager.instance.SetWidth(Random.Range(this._minWidth, this._maxWidth + 1));
        GridManager.instance.SetHeight(Random.Range(this._minHeight, this._maxHeight + 1));
        // GridManager.instance.SetWidth(3);
        // GridManager.instance.SetHeight(3);
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
}
