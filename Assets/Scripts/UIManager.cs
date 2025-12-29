using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _submitButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private RectTransform _actionButtons;
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _playerHealthText;
    [SerializeField] private TextMeshProUGUI _traitorHealthText;
    [SerializeField] private TextMeshProUGUI _ammoPowerUpText;
    [SerializeField] private GameObject _cannonBallSprite;
    [SerializeField] private GameObject _traitorsLineSprite;
    [SerializeField] private TextMeshProUGUI _timePowerUpText;
    [SerializeField] private GameObject _hourglassIcon;
    [SerializeField] private GameObject _rockSprite;
    [SerializeField] private TextMeshProUGUI _healthPowerUpText;
    [SerializeField] private GameObject _heartIcon;
    [SerializeField] private TextMeshProUGUI _hotbarFeedText;
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _traitorHealthBar;

    [SerializeField] private float _baseLevelTextBGWidth = 480f;
    [SerializeField] private float _normalLevelTextBGWidth = 550f;
    [SerializeField] private float _memorizeTextBGWidth = 800f;
    [SerializeField] private float _completeTextBGWidth = 650f;
    [SerializeField] private float _endTextBGWidth = 720f;

    [SerializeField] private List<Image> _bulletBars;
    [SerializeField] private TextMeshProUGUI _bulletsText;
    
    [SerializeField] private float _playerHealthBarSpeed = 3f;
    
    public static UIManager instance;
    
    private RectTransform _topTextBG;
    private const string loadingNextLevel = "Loading Next Level...";

    void Awake() {
        instance = this;
        this._topTextBG = this._topText.GetComponentInParent<Image>().rectTransform;
        this._restartButton.gameObject.SetActive(false);
    }
    
    void Update() {
        // Update Player Health Bar
        var healthPercent = (float)GameManager.instance.player.GetCurrentHealth() / GameManager.instance.player.GetMaxHealth();
        this._playerHealthBar.fillAmount = Mathf.Lerp(this._playerHealthBar.fillAmount, healthPercent, 
            this._playerHealthBarSpeed * Time.deltaTime);
        this._playerHealthBar.color = Color.Lerp(Color.red, Color.green, healthPercent);
    }
    
    public void OnSubmit() {
        Debug.Log("Submit");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        GameManager.instance.player.hasEnded = true;
        // Disable undo and reset after submission
        this._undoButton.interactable = false;
        this._resetButton.interactable = false;
    }

    public void OnUndo() {
        Debug.Log("Undo");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        StartCoroutine(GameManager.instance.player.UndoMove(GameManager.instance.player.TimeToMove));
    }
    
    public void OnReset() {
        Debug.Log("Reset");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        StartCoroutine(GameManager.instance.player.ResetMoves(GameManager.instance.player.TimeToMove));
    }
    
    public void OnRestart() {
        Debug.Log("Restart");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        this._restartButton.gameObject.SetActive(false);
        this._topText.text = loadingNextLevel;
        OnRestartExit(); // change Restart text color back to original color;
        LevelManager.instance.SetLevelCoroutine(StartCoroutine(LevelManager.instance.GenerateLevel()));
    }

    public void OnRestartEnter() {
        TextMeshProUGUI restartText = this._restartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartText.text = "<color=#FF0000>[RESTART]</color>";
    }
    public void OnRestartExit() {
        TextMeshProUGUI restartText = this._restartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartText.text = "<color=#FFB90D>[RESTART]</color>";
    }

    public void UpdateBulletBar (bool enable) {
        this._bulletsText.text = $"({GameManager.instance.pWeaponManager.GetCurrentMagazineCount()}/" + 
                                 $"{GameManager.instance.pWeaponManager.GetMaxMagazineCount()})";
        if (enable) {
            for (var i = 0; i < GameManager.instance.pWeaponManager.GetCurrentMagazineCount(); i++) {
                this._bulletBars[i].enabled = true;
            }
        } else this._bulletBars[GameManager.instance.pWeaponManager.GetCurrentMagazineCount()].enabled = false;
    }
    
    public void UpdatePlayerHealthText() {
        this._playerHealthText.text = $"{GameManager.instance.player.GetCurrentHealth()}%";
    }
    
    public void UpdateTraitorHealth() {
        this._traitorHealthText.text = $"{GameManager.instance.traitor.GetCurrentHealth()}%";
        var healthPercent = (float)GameManager.instance.traitor.GetCurrentHealth() / GameManager.instance.traitor.GetMaxHealth();
        this._traitorHealthBar.fillAmount = healthPercent;
    }
    
    public void UpdateGiveAmmoText() {
        this._cannonBallSprite.SetActive(true);
        this._ammoPowerUpText.text = $"{PowerUpManager.instance.totalAmmo}";
    }
    
    public void EnableTraitorsLineSprite() => this._traitorsLineSprite.SetActive(true);
    
    public void UpdateTimeIncreaseText() {
        this._hourglassIcon.SetActive(true);
        this._timePowerUpText.text = $"{PowerUpManager.instance.totalAddedTime:F2}s";
    }

    public void EnableRockSprite() => this._rockSprite.SetActive(true);
    
    public void UpdateHealthBoostText() {
        this._heartIcon.SetActive(true);
        this._healthPowerUpText.text = $"{PowerUpManager.instance.totalHealPoints}";
    }
    
    public void UpdateHotBarFeedText() {
        this._hotbarFeedText.enabled = true;
        switch (PowerUpManager.instance.powerUp) {
            case PowerUpManager.PowerUp.LineTrace: this._hotbarFeedText.text = "•  Restored Traitor's Wake"; break;
            case PowerUpManager.PowerUp.ClearObstacles: this._hotbarFeedText.text = "•  Cleared all obstacles"; break;
            case PowerUpManager.PowerUp.GiveAmmo: {
                this._hotbarFeedText.text = $"•  Gave {PowerUpManager.instance.ammo} ammo"; break; }
            case PowerUpManager.PowerUp.IncreaseCompleteTime: {
                this._hotbarFeedText.text = $"•  Added {PowerUpManager.instance.addedTime:F2}s"; break; }
            case PowerUpManager.PowerUp.HealthBoost: {
                this._hotbarFeedText.text = $"•  Granted {PowerUpManager.instance.healPoints} health"; break;
            }
        }
    }

    public void DisplayEndScreen() {
        this._topText.text = Player.hasWon ? "<color=#00FF00>YOU WON!</color>" : "<color=#FF0000>*Translation Mismatch*</color>";
        this._topTextBG.sizeDelta = new Vector2(this._endTextBGWidth, this._topTextBG.sizeDelta.y);
        this._restartButton.gameObject.SetActive(true);
        SetActionButtons(false);
    }

    public void DisplayLoadingText() {
        this._topText.text = loadingNextLevel;
    }
    
    public void DisplayLevelText(float level) {
        //Adjust topText size based on the level text
        this._topTextBG.sizeDelta = GameManager.instance.difficulty == GameManager.Difficulty.Normal 
            ? new Vector2(this._normalLevelTextBGWidth, this._topTextBG.sizeDelta.y) 
            : new Vector2(this._baseLevelTextBGWidth, this._topTextBG.sizeDelta.y);
        this._topText.text = $"{{Level {level}: {GameManager.instance.difficulty}}}";
    }

    public IEnumerator DisplayTimeToMemorize() {
        //Adjust topText size based on the timeToMemorize text
        this._topTextBG.sizeDelta = new Vector2(this._memorizeTextBGWidth, this._topTextBG.sizeDelta.y);
        while (GameManager.instance.timeToMemorize > 0f) {
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topText.text = $"{{Memorize The Path: [{GameManager.instance.timeToMemorize:F2}s]}}";
            GameManager.instance.SetTimeToMemorize(GameManager.instance.timeToMemorize - Time.deltaTime);
            yield return null;
        }
        this._topText.text = $"{{Memorize The Path: [{0f:F2}s]}}";
    }
    
    public IEnumerator DisplayTimeToComplete() {
        //Adjust topText size based on the timeToComplete text
        this._topTextBG.sizeDelta = new Vector2(this._completeTextBGWidth, this._topTextBG.sizeDelta.y);
        while (!GameManager.instance.player.hasEnded && GameManager.instance.timeToComplete > 0f) {
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topText.text = $"{{Complete in: [{GameManager.instance.timeToComplete:F2}s]!}}";
            GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete - Time.deltaTime);
            yield return null;
        }
        this._topText.text = $"{{Complete In: [{0f:F2}s]!}}";
        // After timeToComplete is done or the submit button was clicked, set player.isEnded to true 
        GameManager.instance.player.hasEnded = true; 
    }
    
    public void SetActionButtons(bool enable) {
        this._submitButton.interactable = enable;
        this._resetButton.interactable = enable;
        this._undoButton.interactable = enable;
    }

    public void DisableAllPowerUpSprites() {
        this._rockSprite.SetActive(false); 
        this._traitorsLineSprite.SetActive(false); 
        this._hourglassIcon.SetActive(false); 
        this._cannonBallSprite.SetActive(false); 
        this._heartIcon.SetActive(false);
        this._hotbarFeedText.enabled = false;
    }
}

