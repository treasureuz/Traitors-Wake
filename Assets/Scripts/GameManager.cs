using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Player _aiPlayerPrefab;

    [SerializeField] private Vector3 finishPos;
    
    [SerializeField] private int _easyNumOfDirs = 5;
    [SerializeField] private int _normalNumOfDirs = 3;
    [SerializeField] private int _hardNumOfDirs = 2;
    
    [SerializeField] private float _easyAITimeToMove = 0.32f;
    [SerializeField] private float _normalAITimeToMove = 0.18f;
    [SerializeField] private float _hardAITimeToMove = 0.1f;

    [SerializeField] private int _easyMinGrid = 4;
    [SerializeField] private int _easyMaxGrid = 8;

    [SerializeField] private int _normalMinGrid = 8;
    [SerializeField] private int _normalMaxGrid = 12;

    [SerializeField] private int _hardMinGrid = 12;
    [SerializeField] private int _hardMaxGrid = 17;

    public Vector3 FinishPos => this.finishPos;
    public static GameManager instance;
    
    public Player _player { get; private set; }
    public Player _aiPlayer { get; private set; }
    public int numOfDirs {get; private set;}

    public enum Difficulty {
        Easy,
        Normal,
        Hard
    }
    [SerializeField] private Difficulty _difficulty = Difficulty.Easy;
    public Difficulty GetDifficulty => this._difficulty;

    void Awake() {
        instance = this;
        this._aiPlayer = Instantiate(this._aiPlayerPrefab, this._aiPlayerPrefab.SpawnPos, Quaternion.identity);
        this._player = Instantiate(this._playerPrefab, this._playerPrefab.SpawnPos, Quaternion.identity);
    }
    
    void Start() {
        //HandleDifficultySettings();
        
        StartCoroutine(LevelManager.instance.StartLevel(this._aiPlayer.GetComponent<AIManager>()));
    }
    
    private void HandleDifficultySettings() {
        switch (this._difficulty) {
            case Difficulty.Easy: {
                numOfDirs = _easyNumOfDirs; 
                GridManager.instance.SetWidth(Random.Range(this._easyMinGrid, this._easyMaxGrid));
                GridManager.instance.SetHeight(Random.Range(this._easyMinGrid, this._easyMaxGrid));
                this._aiPlayer.aiTimeToMove = this._easyAITimeToMove;
                break;
            }
            case Difficulty.Normal: {
                numOfDirs = _normalNumOfDirs; 
                GridManager.instance.SetWidth(Random.Range(this._normalMinGrid, this._normalMaxGrid));
                GridManager.instance.SetHeight(Random.Range(this._normalMinGrid, this._normalMaxGrid));
                this._aiPlayer.aiTimeToMove = this._normalAITimeToMove;
                break;
            }
            case Difficulty.Hard: {
                numOfDirs = _hardNumOfDirs; 
                GridManager.instance.SetWidth(Random.Range(this._hardMinGrid, this._hardMaxGrid));
                GridManager.instance.SetHeight(Random.Range(this._hardMinGrid, this._hardMaxGrid));
                this._aiPlayer.aiTimeToMove = this._hardAITimeToMove;
                break;
            }
        }
        finishPos = new Vector3(GridManager.instance.Width - 1, GridManager.instance.Height - 1);
    }

    public void IncrementDifficulty() {
        if (this._difficulty == Difficulty.Hard) return;
        this._difficulty += 1;
    }
    public void SetDifficulty(Difficulty diff) {
        this._difficulty = diff;
        HandleDifficultySettings();
    }
}
