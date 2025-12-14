using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _aiPlayerPrefab;
    [SerializeField] private Player _playerPrefab;
    
    [SerializeField] private int easyNumOfDirs = 3;
    [SerializeField] private int normalNumOfDirs = 2;
    [SerializeField] private int hardNumOfDirs = 1;
    
    [SerializeField] private float _easyAITimeToMove = 0.5f;
    [SerializeField] private float _normalAITimeToMove = 0.32f;
    [SerializeField] private float _hardAITimeToMove = 0.18f;

    public static GameManager instance;
    private Player _player;
    private Player _aiPlayer;
    public int numOfDirs {get; private set;}

    public enum Difficulty {
        Easy,
        Normal,
        Hard
    }
    [SerializeField] private Difficulty _difficulty = Difficulty.Easy;
    public Difficulty GetDifficulty => this._difficulty;

    void Start() {
        instance = this;
        this._aiPlayer = Instantiate(this._aiPlayerPrefab, this._aiPlayerPrefab.transform.position, Quaternion.identity);
        this._player = Instantiate(this._playerPrefab, this._playerPrefab.transform.position, Quaternion.identity);
        HandleDifficultySettings();
    }
    
    private void HandleDifficultySettings() {
        switch (this._difficulty) {
            case Difficulty.Easy: {
                numOfDirs = easyNumOfDirs; 
                this._aiPlayer.aiTimeToMove = this._easyAITimeToMove;
                break;
            }
            case Difficulty.Normal: {
                numOfDirs = normalNumOfDirs; 
                this._aiPlayer.aiTimeToMove = this._normalAITimeToMove;
                break;
            }
            case Difficulty.Hard: {
                numOfDirs = hardNumOfDirs; 
                this._aiPlayer.aiTimeToMove = this._hardAITimeToMove;
                break;
            }
        }
    }

    public void SetDifficulty(Difficulty diff) {
        this._difficulty = diff;
        HandleDifficultySettings();
    }
}
