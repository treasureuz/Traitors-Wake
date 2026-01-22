using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Button References")]
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _submitButton;
    [SerializeField] private Button _pauseRestartButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _pauseHomeButton;
    [SerializeField] private Button _endRestartButton;
    [SerializeField] private Button _endHomeButton;
    
    [Header("TMP References")]
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _playerHealthText;
    [SerializeField] private TextMeshProUGUI _traitorHealthText;
    [SerializeField] private TextMeshProUGUI _ammoPowerUpText;
    [SerializeField] private TextMeshProUGUI _timePowerUpText;
    [SerializeField] private TextMeshProUGUI _healthPowerUpText;
    [SerializeField] private TextMeshProUGUI _hotbarFeedText;
    [SerializeField] private TextMeshProUGUI _bulletsText;
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    
    [Header("Top Text Settings")]
    [SerializeField] private float _baseLevelTextBGWidth = 480f;
    [SerializeField] private float _mediumLevelTextBGWidth = 550f;
    [SerializeField] private float _memorizeTextBGWidth = 800f;
    [SerializeField] private float _completeTextBGWidth = 650f;
    [SerializeField] private float _endTextBGWidth = 720f;

    [Header("GameObject References")]
    [SerializeField] private GameObject _cannonBallSprite;
    [SerializeField] private GameObject _traitorsLineSprite;
    [SerializeField] private GameObject _hourglassIcon;
    [SerializeField] private GameObject _rockSprites;
    [SerializeField] private GameObject _heartIcon;
    [SerializeField] private GameObject _mapBorder;
    
    [Header("Image References")]
    [SerializeField] private List<Image> _bulletBars;
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _traitorHealthBar;

    [Header("Others")] 
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _playerHealthBarSpeed = 3f;
    
    public static UIManager instance;
    private RectTransform _topTextBG;
    private SpriteRenderer[] _mapBorderChildren;
    
    private Coroutine _timeToMemorizeCoroutine;
    private Coroutine _timeToCompleteCoroutine;
    
    public bool isMemorizing { get; private set; }
    public bool isCompleting { get; private set; }
    
    private const string loadingLevel = "{Loading Level...}";
    private const string currentScoreText = "Current Score";
    private const string highScoreText = "High Score";
    private const string exitText = "<color=#FFB90D>[Exit]</color>";
    private const string winText = "<color=#00FF00>Traitor Captured!</color>";
    private const string loseText = "<color=#FF0000>*Translation Mismatch*</color>";
    private const string outOfLivesText = "<color=#FF0000>*You Are Out of Lives*</color>";
    private const string deadText = "<color=#FF0000>*DOWN BUT NOT OUT*</color>";

    void Awake() {
        instance = this;
        this._topTextBG = this._topText.GetComponentInParent<Image>().rectTransform;
        this._mapBorderChildren = this._mapBorder.GetComponentsInChildren<SpriteRenderer>();
    }
    
    public void Start() {
        // Reset relevant UI elements 
        ResetCanvasUIAlpha(); // Reset opacity 
        SetActionButtons(false); SetPauseButton(false);
        SetOnPauseButtons(false); // Disable Resume, Restart, Home on start
        SetEndScreenButtons(false);
        // Every time this scene loads, update these UI Elements (based on difficulty)
        UpdatePowerUpsUI(); UpdateScoreText(); // Don't calculate score
    }
    
    void Update() {
        // Update Player Health Bar
        var healthPercent = (float) GameManager.instance.player.GetCurrentHealth() / GameManager.instance.player.GetMaxHealth();
        this._playerHealthBar.fillAmount = Mathf.Lerp(this._playerHealthBar.fillAmount, healthPercent, 
            this._playerHealthBarSpeed * Time.deltaTime);
        this._playerHealthBar.color = Color.Lerp(Color.red, Color.green, healthPercent);
    }
    
    public void OnSubmit() {
        Debug.Log("Submit");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        // Disable action buttons
        SetActionButtons(false);
        GameManager.instance.player.OnPlayerEnded(); // Sets hasEnded to true and updates score
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

    public void OnPause() {
        GameManager.isPaused = true; DimCanvasUI();
        this._pauseButton.interactable = false;
        SetActionButtons(false); // Disable action buttons
        SetOnPauseButtons(true); // Activate Resume, Restart, Home
    }
    
    public void OnResume() {
        StartCoroutine(OnResumeCoroutine());
    }

    private IEnumerator OnResumeCoroutine() {
        SetOnPauseButtons(false); // Disable Resume, Restart, Home
        yield return new WaitForSeconds(0.5f); // Wait before resuming game
        GameManager.isPaused = false; ResetCanvasUIAlpha();
        SetPauseButton(true); // Reenable the pause button
        if (this.isCompleting) SetActionButtons(true); // Reactivate action buttons
    }

    public void OnPauseRestart() {
        // Undo power ups if player restarted
        GameManager.instance.GetPowerUpManagerByDiff().UndoStolenPowerUps();
        DisableGameButtonsOnPause(); OnRestart(); 
    }

    public void OnEndRestart() => OnRestart();
    
    private void OnRestart() {
        Debug.Log("Restart");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        ResetCanvasUIAlpha(); 
        // Calls ResetRunState if player restarted in first level 
        if (LevelManager.instance.GetTotalLevelsCompleted() == 0) LevelManager.hasResetRun = true;
        LevelManager.instance.StopAllCoroutines(true);
        LevelManager.instance.TryStartLevel(); // Restart current level
    }

    public void OnHome() {
        SceneManager.LoadScene("DifficultySelectScene");
        // Undo power ups if player exited
        GameManager.instance.GetPowerUpManagerByDiff().UndoStolenPowerUps();
        LevelManager.instance.StopAllCoroutines(true);
        OnHomeExit(); // Reset home color to original color
        DisableGameButtonsOnPause(); // If GameManager.isPaused, disable pauseGameButtons instead disable endScreenButtons
        ResetCanvasUIAlpha();
    }
    
    public void OnHomeEnter() {
        TextMeshProUGUI homeText = this._pauseHomeButton.GetComponentInChildren<TextMeshProUGUI>();
        homeText.text = "<color=#FF0000>[Exit]</color>";
    }

    public void OnHomeExit() {
        TextMeshProUGUI homeText = this._pauseHomeButton.GetComponentInChildren<TextMeshProUGUI>();
        homeText.text = exitText;
    }

    public void DimCanvasUI() {
        this._canvasGroup.alpha = 0.65f;
        ChangeBGObjectsAlpha();
    }

    private void ResetCanvasUIAlpha() {
        this._canvasGroup.alpha = 1f;
        ChangeBGObjectsAlpha();
    }

    private void ChangeBGObjectsAlpha() {
        foreach (SpriteRenderer s in this._mapBorderChildren) {
            s.color = new Color(s.color.r, s.color.g, s.color.b, this._canvasGroup.alpha);
        }
        foreach (SpriteRenderer s in GridManager.instance.tilesParentChildren) {
            s.color = new Color(s.color.r, s.color.g, s.color.b, this._canvasGroup.alpha);
        }
    }
    
    public void UpdateScoreText() {
        var currentScore = ScoreManager.instance.GetTotalCurrentScore();
        var totalHighScore = ScoreManager.instance.GetTotalHighScore();
        this._currentScoreText.text = $"{currentScoreText}: {currentScore:F2}\n" +
                                      $"{highScoreText}: {totalHighScore:F2}";
    }
    
    public void UpdateBulletBar() {
        this._bulletsText.text = $"({GameManager.instance.pWeaponManager.GetCurrentMagazineCount()}/" + 
                                 $"{GameManager.instance.pWeaponManager.GetMaxMagazineCount()})";
        for (var i = 0; i < GameManager.instance.pWeaponManager.GetCurrentMagazineCount(); i++) {
            this._bulletBars[i].enabled = true; 
        }
        for (var i = GameManager.instance.pWeaponManager.GetMaxMagazineCount() - 1;
             i >= GameManager.instance.pWeaponManager.GetCurrentMagazineCount(); i--) {
            this._bulletBars[i].enabled = false;
        }
    }
    
    public void UpdatePlayerHealthText() {
        this._playerHealthText.text = $"{GameManager.instance.player.GetCurrentHealth()}%";
    }
    
    public void UpdateTraitorHealth() {
        this._traitorHealthText.text = $"{GameManager.instance.traitor.GetCurrentHealth()}%";
        var healthPercent = (float) GameManager.instance.traitor.GetCurrentHealth()/GameManager.instance.traitor.GetMaxHealth();
        this._traitorHealthBar.fillAmount = healthPercent;
    }
    
    public void UpdateGiveAmmoText() {
        this._cannonBallSprite.SetActive(true);
        this._ammoPowerUpText.text = $"{GameManager.instance.GetPowerUpManagerByDiff().totalAmmo}";
    }
    
    public void EnableTraitorsLineSprite() => this._traitorsLineSprite.SetActive(true);
    public void EnableRockSprites() => this._rockSprites.SetActive(true);
    
    public void UpdateTimeIncreaseText() {
        this._hourglassIcon.SetActive(true);
        this._timePowerUpText.text = $"{GameManager.instance.GetPowerUpManagerByDiff().totalAddedTime:F2}s";
    }
    
    public void UpdateHealthBoostText() {
        this._heartIcon.SetActive(true);
        this._healthPowerUpText.text = $"{GameManager.instance.GetPowerUpManagerByDiff().totalHealPoints}";
    }
    
    public void UpdatePowerUpsUI() {
        UpdateHotBarFeedText();
        foreach (PowerUpManager.PowerUp powerUp in GameManager.instance.GetPowerUpManagerByDiff().GetActivatedPowerUps()) {
            switch (powerUp) {
                case PowerUpManager.PowerUp.AmmoSurplus: UpdateGiveAmmoText(); break;
                case PowerUpManager.PowerUp.LineTrace: EnableTraitorsLineSprite(); break;
                case PowerUpManager.PowerUp.ClearObstacles: EnableRockSprites(); break;
                case PowerUpManager.PowerUp.BonusTime: UpdateTimeIncreaseText(); break;
                case PowerUpManager.PowerUp.HealthBoost: UpdateHealthBoostText(); break;
            }
        }
    }
    
    public void DisablePowerUp(PowerUpManager.PowerUp powerUp) {
        switch (powerUp) {
            case PowerUpManager.PowerUp.AmmoSurplus: this._cannonBallSprite.SetActive(false); break;
            case PowerUpManager.PowerUp.LineTrace: this._traitorsLineSprite.SetActive(false); break;
            case PowerUpManager.PowerUp.ClearObstacles: this._rockSprites.SetActive(false); break;
            case PowerUpManager.PowerUp.BonusTime: this._hourglassIcon.SetActive(false); break;
            case PowerUpManager.PowerUp.HealthBoost: this._heartIcon.SetActive(false); break;
        }
    }
    
    public void UpdateHotBarFeedText() {
        this._hotbarFeedText.enabled = true;
        switch (GameManager.instance.GetPowerUpManagerByDiff().powerUp) {
            case PowerUpManager.PowerUp.LineTrace: this._hotbarFeedText.text = "• Restored Traitor's Wake"; break;
            case PowerUpManager.PowerUp.ClearObstacles: this._hotbarFeedText.text = "• Cleared all obstacles"; break;
            case PowerUpManager.PowerUp.AmmoSurplus: {
                this._hotbarFeedText.text = $"• Gave {GameManager.instance.GetPowerUpManagerByDiff().ammo} ammo"; break; }
            case PowerUpManager.PowerUp.BonusTime: {
                this._hotbarFeedText.text = $"• Added {GameManager.instance.GetPowerUpManagerByDiff().addedTime:F2}s"; break; }
            case PowerUpManager.PowerUp.HealthBoost: {
                this._hotbarFeedText.text = $"• Granted {GameManager.instance.GetPowerUpManagerByDiff().healPoints} health"; break;
            }
            case PowerUpManager.PowerUp.None: this._hotbarFeedText.text = ""; break;
        }
    }
    
    public void DisplayEndScreen() {
        this._topTextBG.sizeDelta = new Vector2(this._endTextBGWidth, this._topTextBG.sizeDelta.y);
        Debug.Log($"From DecrementLives: {Player.isOutOfLives}");
        if (Player.hasWon) {
            this._topText.text = winText;
            this._endRestartButton.interactable = false; // Can't restart if won
        } else if (Player.isOutOfLives) {
            this._topText.text = outOfLivesText;
            this._endRestartButton.interactable = false; // Can't restart if out of lives
        } else if (GameManager.instance.player.isDead) this._topText.text = deadText;
        else this._topText.text = loseText;
        DimCanvasUI();
        SetEndScreenButtons(true);
        SetActionButtons(false);
    }

    public void DisplayLoadingText() {
        this._topTextBG.sizeDelta = new Vector2(this._mediumLevelTextBGWidth, this._topTextBG.sizeDelta.y);
        this._topText.text = loadingLevel;
    }
    
    public void DisplayLevelText(float level) {
        // Adjust topText size based on the level text
        this._topTextBG.sizeDelta = GameManager.instance.difficulty == GameManager.Difficulty.Medium 
            ? new Vector2(this._mediumLevelTextBGWidth, this._topTextBG.sizeDelta.y) 
            : new Vector2(this._baseLevelTextBGWidth, this._topTextBG.sizeDelta.y);
        this._topText.text = $"{{Level {level}: {GameManager.instance.difficulty}}}";
    }

    private IEnumerator DisplayTimeToMemorize() {
        if (this.isMemorizing) yield break;
        this.isMemorizing = true;
        // Adjust topText size based on the timeToMemorize text
        this._topTextBG.sizeDelta = new Vector2(this._memorizeTextBGWidth, this._topTextBG.sizeDelta.y);
        GameManager.instance.SetTimeToMemorize(GameManager.instance.GetTimeToMemorizeByDiff()); // Resets timeToMemorize
        while (GameManager.instance.timeToMemorize > 0f) {
            if (GameManager.isPaused) yield return new WaitUntil(() => !GameManager.isPaused);
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topText.text = $"{{Memorize The Path: [{GameManager.instance.timeToMemorize:F2}s]}}";
            GameManager.instance.SetTimeToMemorize(GameManager.instance.timeToMemorize - Time.deltaTime);
            yield return null;
        }
        this.isMemorizing = false;
        this._topText.text = $"{{Memorize The Path: [{0f:F2}s]}}";
    }

    public void StartTimeToMemorizeCoroutine() {
        if (this._timeToMemorizeCoroutine != null) StopCoroutine(this._timeToMemorizeCoroutine);
        this._timeToMemorizeCoroutine = StartCoroutine(DisplayTimeToMemorize());
    }

    public void TryStopTimeToMemorizeCoroutine() {
        if (this._timeToMemorizeCoroutine == null) return;
        StopCoroutine(this._timeToMemorizeCoroutine);
        this._timeToMemorizeCoroutine = null;
        this.isMemorizing = false;
    }
    
    private IEnumerator DisplayTimeToComplete() {
        if (this.isCompleting) yield break;
        this.isCompleting = true;
        // Adjust topText size based on the timeToComplete text
        this._topTextBG.sizeDelta = new Vector2(this._completeTextBGWidth, this._topTextBG.sizeDelta.y);
        GameManager.instance.SetTimeToComplete(GameManager.instance.GetTimeToCompleteByDiff()); // Resets timeToComplete
        while (!GameManager.instance.player.hasEnded && GameManager.instance.timeToComplete > 0f) {
            if (GameManager.isPaused) yield return new WaitUntil(() => !GameManager.isPaused);
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topText.text = $"{{Complete in: [{GameManager.instance.timeToComplete:F2}s]!}}";
            GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete - Time.deltaTime);
            yield return null;
        }
        this.isCompleting = false;
        // After timeToComplete is done, call OnPlayerEnded
        if (GameManager.instance.player.hasEnded) yield break;
        this._topText.text = $"{{Complete In: [{0f:F2}s]!}}";
        GameManager.instance.player.OnPlayerEnded(); // Sets hasEnded to true and updates score
    }
    
    public void StartTimeToCompleteCoroutine() {
        if (this._timeToCompleteCoroutine != null) StopCoroutine(this._timeToCompleteCoroutine);
        this._timeToCompleteCoroutine = StartCoroutine(DisplayTimeToComplete());
    }
    
    public void TryStopTimeToCompleteCoroutine() {
        if (this._timeToCompleteCoroutine == null) return;
        StopCoroutine(this._timeToCompleteCoroutine);
        this._timeToCompleteCoroutine = null;
        this.isCompleting = false;
    }
    
    public void SetActionButtons(bool enable) {
        this._submitButton.interactable = enable;
        this._resetButton.interactable = enable;
        this._undoButton.interactable = enable;
    }

    private void DisableGameButtonsOnPause() {
        if (GameManager.isPaused) {
            GameManager.isPaused = false;
            SetOnPauseButtons(false);
        } else SetEndScreenButtons(false);
    }
    public void SetPauseButton(bool enable) => this._pauseButton.interactable = enable;
    private void SetOnPauseButtons(bool enable) => this._resumeButton.transform.parent.gameObject.SetActive(enable);
    private void SetEndScreenButtons(bool enable) => this._endRestartButton.transform.parent.gameObject.SetActive(enable);
    
    public void DisableAllPowerUpSprites() {
        this._rockSprites.SetActive(false); 
        this._traitorsLineSprite.SetActive(false); 
        this._hourglassIcon.SetActive(false); 
        this._cannonBallSprite.SetActive(false); 
        this._heartIcon.SetActive(false);
        this._hotbarFeedText.enabled = false;
    }
}

