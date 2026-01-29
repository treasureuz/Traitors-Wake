using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersSettingsManager : MonoBehaviour {
    [SerializeField] private int _maxLivesCount = 5;
    
    public static PlayersSettingsManager instance;

    private int _easyPlayerCurrentHealth;
    private int _mediumPlayerCurrentHealth;
    private int _hardPlayerCurrentHealth;
    private int _easyPlayerCurrentMagazineCount;
    private int _mediumPlayerCurrentMagazineCount;
    private int _hardPlayerCurrentMagazineCount;
  
    private int _easyTraitorCurrentHealth;
    private int _mediumTraitorCurrentHealth;
    private int _hardTraitorCurrentHealth;
    private int _easyTraitorCurrentMagazineCount;
    private int _mediumTraitorCurrentMagazineCount;
    private int _hardTraitorCurrentMagazineCount;
    
    private int _currentLivesCount;
    public bool hasPlayerWon { get; private set; }
    public bool isPlayerOOL { get; private set; }
        
    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    
    void OnEnable() {
        this._currentLivesCount = this._maxLivesCount; // this only happens once
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // If scene is GameScene, apply player settings
        if (scene.name == "GameScene") Begin();
    }
    private void Begin() => ApplyPlayersSettings();

    public void ApplyPlayersSettings() {
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: {
                GameManager.instance.player.SetCurrentHealth(this._easyPlayerCurrentHealth); 
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount(this._easyPlayerCurrentMagazineCount);
                GameManager.instance.traitor.SetCurrentHealth(this._easyTraitorCurrentHealth); 
                GameManager.instance.tWeaponManager.SetCurrentMagazineCount(this._easyTraitorCurrentMagazineCount);
                break;
            }
            case GameManager.Difficulty.Medium: {
                GameManager.instance.player.SetCurrentHealth(this._mediumPlayerCurrentHealth); 
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount(this._mediumPlayerCurrentMagazineCount);
                GameManager.instance.traitor.SetCurrentHealth(this._mediumTraitorCurrentHealth); 
                GameManager.instance.tWeaponManager.SetCurrentMagazineCount(this._mediumTraitorCurrentMagazineCount);
                break;
            }
            case GameManager.Difficulty.Hard: {
                GameManager.instance.player.SetCurrentHealth(this._hardPlayerCurrentHealth); 
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount(this._hardPlayerCurrentMagazineCount);
                GameManager.instance.traitor.SetCurrentHealth(this._hardTraitorCurrentHealth); 
                GameManager.instance.tWeaponManager.SetCurrentMagazineCount(this._hardTraitorCurrentMagazineCount);
                break;
            }
        }
        UIManager.instance.UpdatePlayerHealthText(); 
        UIManager.instance.UpdateTraitorHealth();
        UIManager.instance.UpdateBulletBar();
    }
    
    public void SavePlayersSettings() {
        this.hasPlayerWon = Player.hasWon;
        this.isPlayerOOL = Player.isOutOfLives;
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: {
                this._easyPlayerCurrentHealth = GameManager.instance.player.GetCurrentHealth(); 
                this._easyPlayerCurrentMagazineCount = GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
                this._easyTraitorCurrentHealth = GameManager.instance.traitor.GetCurrentHealth(); 
                this._easyTraitorCurrentMagazineCount = GameManager.instance.tWeaponManager.GetCurrentMagazineCount();
                break;
            }
            case GameManager.Difficulty.Medium: {
                this._mediumPlayerCurrentHealth = GameManager.instance.player.GetCurrentHealth(); 
                this._mediumPlayerCurrentMagazineCount = GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
                this._mediumTraitorCurrentHealth = GameManager.instance.traitor.GetCurrentHealth(); 
                this._mediumTraitorCurrentMagazineCount = GameManager.instance.tWeaponManager.GetCurrentMagazineCount();
                break;
            }
            case GameManager.Difficulty.Hard: {
                this._hardPlayerCurrentHealth = GameManager.instance.player.GetCurrentHealth(); 
                this._hardPlayerCurrentMagazineCount = GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
                this._hardTraitorCurrentHealth = GameManager.instance.traitor.GetCurrentHealth();
                this._hardTraitorCurrentMagazineCount = GameManager.instance.tWeaponManager.GetCurrentMagazineCount();
                break;
            }
        }
    }
    
    public int GetCurrentLivesCount() => this._currentLivesCount;
    public int GetMaxLivesCount() => this._maxLivesCount;
    public void DecrementCurrentLivesCount() => --this._currentLivesCount;
    public void ResetSettings() {
        this._currentLivesCount = this._maxLivesCount;
        Player.isOutOfLives = false; isPlayerOOL = false;
        hasPlayerWon = false;
    } 
}