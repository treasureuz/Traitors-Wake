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
    [SerializeField] private Button _nextDiffButton;
    [SerializeField] private Button _endHomeButton;
    
    [Header("TMP References")]
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _playerHealthText;
    [SerializeField] private TextMeshProUGUI _traitorText;
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
    [SerializeField] private GameObject _winOOLScreenPrefab;
    [SerializeField] private GameObject _cannonBallSprite;
    [SerializeField] private GameObject _traitorsLineSprite;
    [SerializeField] private GameObject _hourglassIcon;
    [SerializeField] private GameObject _rockSprites;
    [SerializeField] private GameObject _heartIcon;
    [SerializeField] private GameObject _mapBorder;
    
    [Header("Sprite References")]
    [SerializeField] private Sprite _traitorBehindBars;
    [SerializeField] private Sprite _traitorBrokenBars;
    
    [Header("Image References")]
    [SerializeField] private List<Image> _livesIcons;
    [SerializeField] private List<Image> _bulletBars;
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _traitorHealthBar;

    [Header("Canvas References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Canvas _endCanvas;
    
    [Header("Others")] 
    [SerializeField] private float _playerHealthBarSpeed = 3f;
    
    public static UIManager instance;
    
    private GameObject _spawnedWinOOLScreen;
    private TextMeshProUGUI _bountyText;
    private Image _winOOLImage;
    private RectTransform _topTextBG;
    private SpriteRenderer[] _mapBorderChildren;
    
    private Coroutine _timeToMemorizeCoroutine;
    private Coroutine _timeToCompleteCoroutine;
    
    public bool isMemorizing { get; private set; }
    public bool isCompleting { get; private set; }
    
    private const string loadingLevel = "{Loading Level...}";
    private const string currentScoreText = "Current Score";
    private const string highScoreText = "High Score";
    private const string restartText = "<color=#FFB90D><size=35px>[Restart]</size></color>";
    private const string exitText = "<color=#FFB90D>[Exit]</color>";
    private const string traitorHealthText = "<size=28px>{Traitor's Health}</size>";
    private const string tillerHealthText = "<size=27px>{Traitor's Tiller Health}</size>";
    private const string sternHealthText = "<size=27px>{Traitor's Stern Health}</size>";
    private const string tillerEndText = "<color=#00FF00>*YOU DISABLED HIS SHIP!*</color>"; 
    private const string sternEndText = "<color=#00FF00>*YOU BLEW OUT THE STERN!*</color>";
    private const string bountyWinText = "BOUNTY <color=#00FF00>CLAIMED!</color>";
    private const string bountyOOLText = "BOUNTY <color=#FF0000>ESCAPED!</color>";
    private const string topWinText = "<color=#00FF00><size=45px>*TRAITOR CAPTURED!*</size></color>";
    private const string loseText = "<color=#FF0000>*TRANSLATION MISMATCH*</color>";
    private const string deadText = "<color=#FF0000>*DOWN BUT NOT OUT*</color>";
    private const string topOOLText = "<color=#FF0000>*YOU ARE OUT OF LIVES*</color>";
    private const string diffCompleteText = "<color=#00FF00><size=45px>*DIFFICULTY COMPLETE!*</size></color>";

    void Awake() {
        instance = this;
        this._topTextBG = this._topText.GetComponentInParent<Image>().rectTransform;
        this._mapBorderChildren = this._mapBorder.GetComponentsInChildren<SpriteRenderer>();
    }
    
    public void Start() {
        // Reset relevant UI elements 
        ResetCanvasUIAlpha(); // Reset opacity
        SetPauseButton(false); SetOnPauseButtons(false); // Disable Resume, Restart, Home on start
        SetActionButtons(false); SetEndScreenButtons(false);
        // Every time this scene loads, update these UI Elements 
        UpdateLivesIcons(); UpdatePowerUpsUI(); UpdateScoreText(); // Don't calculate score
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
        // this._pauseButton.interactable = false;
        // SetActionButtons(false); // Disable action buttons
        SetOnPauseButtons(true); // Activate Resume, Restart, Home
    }
    
    public void OnResume() => StartCoroutine(OnResumeCoroutine());
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
        ScoreManager.instance.HandleScorePenalty(); // If player restarted level
        GameManager.isPaused = false; // Set isPaused to false on restart
        OnRestart(); OnPauseRestartExit(); // Reset restart color to original color
    }
    public void OnPauseRestartEnter() {
        TextMeshProUGUI restartTMP = this._pauseRestartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartTMP.text = "<color=#FFB90D><size=38.2px>[Restart]</size></color>";
    }
    
    public void OnPauseRestartExit() {
        TextMeshProUGUI restartTMP = this._pauseRestartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartTMP.text = $"{restartText}<color=#FF0000><size=27.5px>(-{ScoreManager.instance.GetPenaltyPoints()})</size></color>";
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

    public void OnNextDifficulty() {
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        // endRestartButton takes over nextDiffButton
        this._nextDiffButton.gameObject.SetActive(false);
        this._endRestartButton.gameObject.SetActive(true);
        GameManager.instance.IncrementDifficulty(); // calls HandleDifficultySettings
        if (LevelManager.instance.GetCurrentLevelByDiff() == 0) LevelManager.hasResetRun = true; // calls ResetRunState
        LevelManager.instance.TryStartLevel(); // Start next level with higher difficulty
    }
    
    public void OnHome() {
        SceneManager.LoadScene("DifficultySelectScene");
        // Undo power ups if player exited
        LevelManager.instance.StopAllCoroutines(true);
        if (GameManager.isPaused) GameManager.instance.GetPowerUpManagerByDiff().UndoStolenPowerUps();
        ScoreManager.instance.HandleScorePenalty(); // If player exit while in level
        GameManager.isPaused = false; // Set isPaused to false when player exits
        ResetCanvasUIAlpha(); OnPauseHomeExit(); OnEndHomeExit(); // Reset home color to original color
    }
    
    public void OnHomeEnter() {
        TextMeshProUGUI homeText = this._pauseHomeButton.GetComponentInChildren<TextMeshProUGUI>();
        homeText.text = "<color=#FF0000>[Exit]</color>";
    }

    public void OnPauseHomeExit() {
        TextMeshProUGUI homeText = this._pauseHomeButton.GetComponentInChildren<TextMeshProUGUI>();
        homeText.text = $"{exitText}<color=#FF0000><size=30px>(-{ScoreManager.instance.GetPenaltyPoints()})</size></color>";
    }
    
    public void OnEndHomeExit() {
        if (this._spawnedWinOOLScreen) Destroy(this._spawnedWinOOLScreen);
        TextMeshProUGUI homeText = this._endHomeButton.GetComponentInChildren<TextMeshProUGUI>();
        homeText.text = exitText;
    }
    
    public void DisplayEndScreen() {
        DimCanvasUI(); this._topTextBG.sizeDelta = new Vector2(this._endTextBGWidth, this._topTextBG.sizeDelta.y);
        if (Player.hasWon) HandlePlayerWinScreen(); // Also works for Traitor's death 
        else if (Player.isOutOfLives) HandlePlayerOOLScreen();
        else if (GameManager.instance.player.isDead) {
            this._topText.text = deadText; SetEndScreenButtons(true);
        } else if (LevelManager.instance.isDifficultyComplete) HandleDiffCompleteScreen(); // Checks for isShipDestroyed
        else {
            this._topText.text = loseText; SetEndScreenButtons(true);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleBasePlayerEndScreen() {
        this._endRestartButton.interactable = false; // Can't restart if player won/OOL
        this._spawnedWinOOLScreen = Instantiate(this._winOOLScreenPrefab, this._endCanvas.transform, false);
        this._bountyText = this._spawnedWinOOLScreen.GetComponentInChildren<TextMeshProUGUI>();
        this._winOOLImage = this._spawnedWinOOLScreen.transform.Find("Win_OOLScreenBG/Win_OOLImageBG/Win_OOLImage").GetComponent<Image>();
        Button exit = this._spawnedWinOOLScreen.GetComponentInChildren<Button>();
        exit.onClick.AddListener(OnHome);
    }
    
    private void HandlePlayerWinScreen() {
        HandleBasePlayerEndScreen();
        this._topText.text = topWinText; 
        this._bountyText.text = bountyWinText;
        this._winOOLImage.sprite = this._traitorBehindBars;
    }
    
    private void HandlePlayerOOLScreen() {
        HandleBasePlayerEndScreen(); 
        this._topText.text = topOOLText; 
        this._bountyText.text = bountyOOLText;
        this._winOOLImage.sprite = this._traitorBrokenBars;
    }
    
    private void HandleDiffCompleteScreen() {
        LevelManager.instance.isDifficultyComplete = false;
        if (GameManager.instance.traitor.isShipDestroyed) OnTraitorShipDestroyed();
        else this._topText.text = diffCompleteText;
        // nextDiffButton takes over endRestartButton
        this._endRestartButton.gameObject.SetActive(false);
        this._nextDiffButton.gameObject.SetActive(true);
        SetEndScreenButtons(true); 
    }
    
    private void OnTraitorShipDestroyed() {
        switch(GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this._topText.text = sternEndText; break;
            case GameManager.Difficulty.Medium: this._topText.text = tillerEndText; break;
        }
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
    
    public void UpdateScoreText() {
        var currentScore = ScoreManager.instance.GetTotalCurrentScore();
        var totalHighScore = ScoreManager.instance.GetTotalHighScore();
        this._currentScoreText.text = $"{currentScoreText}: {currentScore:F2}\n" +
                                      $"{highScoreText}: {totalHighScore:F2}";
    }
    
    public void UpdateLivesIcons() {
        for (var i = 0; i < PlayersSettingsManager.instance.GetCurrentLivesCount(); i++) {
            this._livesIcons[i].enabled = true;
        }
        for (var i = PlayersSettingsManager.instance.GetMaxLivesCount() - 1;
             i >= PlayersSettingsManager.instance.GetCurrentLivesCount(); i--) {
            this._livesIcons[i].enabled = false;
        }
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
        switch (GameManager.instance.difficulty) {
            case GameManager.Difficulty.Easy: this._traitorText.text = sternHealthText; break; // Traitor's Ship - Stern
            case GameManager.Difficulty.Medium: this._traitorText.text = tillerHealthText; break; // Traitor's Ship - Tiller
            case GameManager.Difficulty.Hard: this._traitorText.text = traitorHealthText; break; // Traitor's Health
        }
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
        if (GameManager.instance.GetPowerUpManagerByDiff().GetActivatedPowerUps().Contains(PowerUpManager.PowerUp.AmmoSurplus)){
            UpdateGiveAmmoText();
        } else this._cannonBallSprite.SetActive(false);
        if (GameManager.instance.GetPowerUpManagerByDiff().GetActivatedPowerUps().Contains(PowerUpManager.PowerUp.BonusTime)){
            UpdateTimeIncreaseText();
        } else this._hourglassIcon.SetActive(false);
        if (GameManager.instance.GetPowerUpManagerByDiff().GetActivatedPowerUps().Contains(PowerUpManager.PowerUp.HealthBoost)){
            UpdateHealthBoostText();
        } else this._heartIcon.SetActive(false);
        this._traitorsLineSprite.SetActive(GameManager.instance.GetPowerUpManagerByDiff().GetActivatedPowerUps()
            .Contains(PowerUpManager.PowerUp.TraitorsWake));
        this._rockSprites.SetActive(GameManager.instance.GetPowerUpManagerByDiff().GetActivatedPowerUps()
            .Contains(PowerUpManager.PowerUp.ClearObstacles));
    }
    
    public void UpdateHotBarFeedText() {
        this._hotbarFeedText.enabled = true;
        switch (GameManager.instance.GetPowerUpManagerByDiff().powerUp) {
            case PowerUpManager.PowerUp.TraitorsWake: this._hotbarFeedText.text = "• Restored Traitor's Wake"; break;
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
    
    public void DimCanvasUI() {
        this._canvasGroup.alpha = 0.6f;
        this._canvasGroup.interactable = false;
        ChangeBGObjectsAlpha();
    }

    private void ResetCanvasUIAlpha() {
        this._canvasGroup.alpha = 1f;
        this._canvasGroup.interactable = true;
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
    
    public void SetActionButtons(bool enable) {
        this._submitButton.interactable = enable;
        this._resetButton.interactable = enable;
        this._undoButton.interactable = enable;
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

