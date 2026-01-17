using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersDataManager : MonoBehaviour {
    public static PlayersDataManager instance;

    private int _currentPlayerHealth;
    private int _currentPlayerMagazineCount;
  
    private int _currentTraitorHealth;
    private int _currentTraitorMagazineCount;
    
    void Awake() {
        if (instance) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // If scene is GameScene, apply player settings
        if (scene.name == "GameScene") Begin();
    }
    private void Begin() => ApplyPlayersData();

    public void ApplyPlayersData() {
        GameManager.instance.player.SetCurrentHealth(this._currentPlayerHealth);
        GameManager.instance.pWeaponManager.SetCurrentMagazineCount(this._currentPlayerMagazineCount);
        GameManager.instance.traitor.SetCurrentHealth(this._currentTraitorHealth);
        GameManager.instance.tWeaponManager.SetCurrentMagazineCount(this._currentTraitorMagazineCount);
        UIManager.instance.UpdatePlayerHealthText(); 
        UIManager.instance.UpdateTraitorHealth();
        UIManager.instance.UpdateBulletBar();
    }
    
    public void SavePlayersData() {
        this._currentPlayerHealth = GameManager.instance.player.GetCurrentHealth();
        this._currentPlayerMagazineCount = GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
        this._currentTraitorHealth = GameManager.instance.traitor.GetCurrentHealth();
        this._currentTraitorMagazineCount = GameManager.instance.tWeaponManager.GetCurrentMagazineCount();
    }
}
