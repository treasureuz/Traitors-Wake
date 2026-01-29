using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Traitor _traitorPrefab;
    [SerializeField] private Vector2Int _finishPos;
    
    [Header("Number of Up/Right Directions")]
    [SerializeField] private int _easyNumOfDirs = 5;
    [SerializeField] private int _mediumNumOfDirs = 3;
    [SerializeField] private int _hardNumOfDirs = 2;
    
    [Header("Min/Max Grid Numbers")]
    [SerializeField] private int _easyWidth = 7;
    [SerializeField] private int _easyHeight = 5;
    [SerializeField] private int _mediumWidth = 8;
    [SerializeField] private int _mediumHeight = 6;
    [SerializeField] private int _hardWidth = 9;
    [SerializeField] private int _hardHeight = 7;
    
    [Header("Min/Max Number of Chests/Obstacles")]
    [SerializeField] private int _easyMinNumOfChests = 1;
    [SerializeField] private int _easyMaxNumOfChests = 2;
    [SerializeField] private int _easyMinNumOfObstacles = 2;
    [SerializeField] private int _easyMaxNumOfObstacles = 3;
    [SerializeField] private int _mediumMinNumOfChests = 2;
    [SerializeField] private int _mediumMaxNumOfChests = 3;
    [SerializeField] private int _mediumMinNumOfObstacles = 3;
    [SerializeField] private int _mediumMaxNumOfObstacles = 4;
    [SerializeField] private int _hardMinNumOfChests = 3;
    [SerializeField] private int _hardMaxNumOfChests = 4;
    [SerializeField] private int _hardMinNumOfObstacles = 4;
    [SerializeField] private int _hardMaxNumOfObstacles = 5;
    
    [Header("Players Max Health")]
    [SerializeField] private int _easyPlayerMaxHealth = 70;
    [SerializeField] private int _easyTraitorMaxHealth = 200;
    [SerializeField] private int _mediumPlayerMaxHealth = 85;
    [SerializeField] private int _mediumTraitorMaxHealth = 175;
    [SerializeField] private int _hardPlayerMaxHealth = 100;
    [SerializeField] private int _hardTraitorMaxHealth = 150;
    
    [Header("Time To Memorize")]
    [SerializeField] private float _easyTimeToMemorize = 3.5f;
    [SerializeField] private float _mediumTimeToMemorize = 3f;
    [SerializeField] private float _hardTimeToMemorize = 2.3f;
    
    [Header("Time To Complete")]
    [SerializeField] private float _easyTimeToComplete = 11.8f;
    [SerializeField] private float _mediumTimeToComplete = 12.5f;
    [SerializeField] private float _hardTimeToComplete = 13.2f;
    
    [Header("Player Time To Move")]
    [SerializeField] private float _easyPlayerTimeToMove = 0.3f;
    [SerializeField] private float _mediumPlayerTimeToMove = 0.28f;
    [SerializeField] private float _hardPlayerTimeToMove = 0.25f;
    
    [Header("Traitor Time To Move")]
    [SerializeField] private float _easyTraitorTimeToMove = 0.42f;
    [SerializeField] private float _mediumTraitorTimeToMove = 0.35f;
    [SerializeField] private float _hardTraitorTimeToMove = 0.3f;

    [Header("Traitor Min/Max Time Between Shots")] 
    [SerializeField] private float _easyMinTimeBetweenShots = 3.5f;
    [SerializeField] private float _easyMaxTimeBetweenShots = 8f;
    [SerializeField] private float _mediumMinTimeBetweenShots = 3f;
    [SerializeField] private float _mediumMaxTimeBetweenShots = 7f;
    [SerializeField] private float _hardMinTimeBetweenShots = 2.5f;
    [SerializeField] private float _hardMaxTimeBetweenShots = 6f;
    
    [Header("Easy/Medium Reset Count")]
    [SerializeField] private int _maxEasyResetCount = 2;
    [SerializeField] private int _maxMediumResetCount = 1;
    
    public Vector2Int FinishPos => this._finishPos;
    public static GameManager instance;
        
    public Player player { get; private set; }
    public Traitor traitor { get; private set; }
    public PWeaponManager pWeaponManager { get; private set; }
    public TWeaponManager tWeaponManager { get; private set; }
    
    private PowerUpManager _easyPowerUpManager;
    private PowerUpManager _mediumPowerUpManager;
    private PowerUpManager _hardPowerUpManager;
    
    public int numOfDirs {get; private set;}
    public int numOfChests { get; private set; }
    public int numOfObstacles { get; private set; }
    public int easyResetCount { get; private set; }
    public int mediumResetCount { get; private set; }
    public float timeToMemorize { get; private set; }
    public float timeToComplete { get; private set; }

    public static bool isPaused;
    
    public enum Difficulty {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    public Difficulty difficulty { get; private set; } 

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        this.easyResetCount = this._maxEasyResetCount;
        this.mediumResetCount = this._maxMediumResetCount;
    }
    
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        // If scene is GameScene, setup references and start levels
        if (scene.name == "GameScene") Begin();
    }

    private void Begin() {
        SetupReferences();
        HandleDifficultySettings();
        LevelManager.instance.TryStartLevel();
    }

    private void SetupReferences() {
        this._easyPowerUpManager = this.transform.Find("EasyPowerUpManager").GetComponent<PowerUpManager>();
        this._mediumPowerUpManager = this.transform.Find("MediumPowerUpManager").GetComponent<PowerUpManager>();
        this._hardPowerUpManager = this.transform.Find("HardPowerUpManager").GetComponent<PowerUpManager>();
        this.player = Instantiate(this._playerPrefab, PlayersManager.SpawnPosV3(), Quaternion.identity);
        this.player.gameObject.SetActive(false);
        this.traitor = Instantiate(this._traitorPrefab, PlayersManager.SpawnPosV3(), Quaternion.identity);
        this.traitor.gameObject.SetActive(false);
        this.pWeaponManager = this.player.GetComponentInChildren<PWeaponManager>();
        this.tWeaponManager = this.traitor.GetComponentInChildren<TWeaponManager>();
    }

    public void EnablePlayers() {
        this.player.gameObject.SetActive(true);
        this.traitor.gameObject.SetActive(true);
    }
    
    private void HandleDifficultySettings() {
        switch (this.difficulty) {
            case Difficulty.Easy: {
                this.numOfDirs = this._easyNumOfDirs; 
                this.numOfChests = Random.Range(this._easyMinNumOfChests, this._easyMaxNumOfChests + 1);
                this.numOfObstacles = Random.Range(this._easyMinNumOfObstacles, this._easyMaxNumOfObstacles + 1);
                GridManager.instance.SetWidth(this._easyWidth);
                GridManager.instance.SetHeight(this._easyHeight);
                this.player.SetMaxHealth(this._easyPlayerMaxHealth);
                this.traitor.SetMaxHealth(this._easyTraitorMaxHealth);
                this.timeToMemorize = this._easyTimeToMemorize;
                this.timeToComplete = this._easyTimeToComplete;
                this.player.SetTimeToMove(this._easyPlayerTimeToMove);
                this.traitor.SetTimeToMove(this._easyTraitorTimeToMove);
                this.tWeaponManager.SetMinTimeBetweenShots(this._easyMinTimeBetweenShots);
                this.tWeaponManager.SetMaxTimeBetweenShots(this._easyMaxTimeBetweenShots);
                break;
            }
            case Difficulty.Medium: {
                this.numOfDirs = this._mediumNumOfDirs; 
                this.numOfChests = Random.Range(this._mediumMinNumOfChests, this._mediumMaxNumOfChests + 1);
                this.numOfObstacles = Random.Range(this._mediumMinNumOfObstacles, this._mediumMaxNumOfObstacles + 1);
                GridManager.instance.SetWidth(this._mediumWidth);
                GridManager.instance.SetHeight(this._mediumHeight);
                this.player.SetMaxHealth(this._mediumPlayerMaxHealth);
                this.traitor.SetMaxHealth(this._mediumTraitorMaxHealth);
                this.timeToMemorize = this._mediumTimeToMemorize;
                this.timeToComplete = this._mediumTimeToComplete;
                this.player.SetTimeToMove(this._mediumPlayerTimeToMove);
                this.traitor.SetTimeToMove(this._mediumTraitorTimeToMove);
                this.tWeaponManager.SetMinTimeBetweenShots(this._mediumMinTimeBetweenShots);
                this.tWeaponManager.SetMaxTimeBetweenShots(this._mediumMaxTimeBetweenShots);
                break;
            }
            case Difficulty.Hard: {
                this.numOfDirs = this._hardNumOfDirs; 
                this.numOfChests = Random.Range(this._hardMinNumOfChests, this._hardMaxNumOfChests + 1);
                this.numOfObstacles = Random.Range(this._hardMinNumOfObstacles, this._hardMaxNumOfObstacles + 1);
                GridManager.instance.SetWidth(this._hardWidth);
                GridManager.instance.SetHeight(this._hardHeight);
                this.player.SetMaxHealth(this._hardPlayerMaxHealth);
                this.traitor.SetMaxHealth(this._hardTraitorMaxHealth);
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
    
    public void HandleGameEnd() {
        LevelManager.instance.StopAllCoroutines(true);
        UIManager.instance.DisplayEndScreen();
    }

    public void IncrementDifficulty() {
        SetDifficulty(++this.difficulty);
        HandleDifficultySettings();
    }
    
    public void SetDifficulty(Difficulty diff) => this.difficulty = diff;
    public void SetTimeToComplete(float time) => this.timeToComplete = time;
    public void SetTimeToMemorize(float time) => this.timeToMemorize = time;

    public float GetTimeToMemorizeByDiff() {
        return this.difficulty switch {
            Difficulty.Easy => this._easyTimeToMemorize,
            Difficulty.Medium => this._mediumTimeToMemorize,
            Difficulty.Hard => this._hardTimeToMemorize,
            _ => 0f
        };
    }
    
    public float GetTimeToCompleteByDiff() {
        return this.difficulty switch {
            Difficulty.Easy => this._easyTimeToComplete,
            Difficulty.Medium => this._mediumTimeToComplete,
            Difficulty.Hard => this._hardTimeToComplete,
            _ => 0f
        };
    }

    public int GetMaxResetCount(Difficulty diff) {
        return diff switch {
            Difficulty.Easy => this._maxEasyResetCount,
            Difficulty.Medium => this._maxMediumResetCount,
            _ => 0
        };
    }
    public void DecrementResetCount(Difficulty diff) {
        switch (this.difficulty) {
            case Difficulty.Easy: --this.easyResetCount; break;
            case Difficulty.Medium: -- this.mediumResetCount; break;
        }
    }
    public void ResetResetCounts() {
        this.easyResetCount = this._maxEasyResetCount;
        this.mediumResetCount = this._maxMediumResetCount;
    }
    
    public PowerUpManager GetPowerUpManagerByDiff() {
        return this.difficulty switch {
            Difficulty.Easy => this._easyPowerUpManager,
            Difficulty.Medium => this._mediumPowerUpManager,
            Difficulty.Hard => this._hardPowerUpManager,
            _ => null
        };
    }
}