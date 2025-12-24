using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Image _healthBar;

    [SerializeField] private float _baseLevelTextBGWidth = 480f;
    [SerializeField] private float _normalLevelTextBGWidth = 550f;
    [SerializeField] private float _memorizeTextBGWidth = 800f;
    [SerializeField] private float _completeTextBGWidth = 650f;
    [SerializeField] private float _failedTextBGWidth = 720f;

    [SerializeField] private List<Image> _bulletBars;
    [SerializeField] private TextMeshProUGUI _bulletsText;
    
    [SerializeField] private float _healthBarSpeed = 3f;
    
    public static UIManager instance;
    
    private RectTransform _topTextBGTransform;
    private const string loadingNextLevel = "Loading Next Level...";

    void Awake() {
        instance = this;
        this._topTextBGTransform = this._topText.GetComponentInParent<Image>().rectTransform;
        this._restartButton.gameObject.SetActive(false);
    }

    void Update() {
        // Update Health Bar
        var healthPercent = (float)GameManager.instance.player.GetCurrentHealth() / GameManager.instance.player.GetMaxHealth();
        this._healthBar.fillAmount = Mathf.Lerp(this._healthBar.fillAmount, healthPercent, this._healthBarSpeed * Time.deltaTime);
        this._healthBar.color = Color.Lerp(Color.red, Color.green, healthPercent);
    }
    
    public void OnSubmit() {
        Debug.Log($"Submit: {GameManager.instance.player.hasEnded}");
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
        StartCoroutine(LevelManager.instance.GenerateLevel());
    }

    public void OnRestartEnter() {
        TextMeshProUGUI restartText = this._restartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartText.text = "<color=#FF0000>[RESTART]</color>";
    }
    
    public void OnRestartExit() {
        TextMeshProUGUI restartText = this._restartButton.GetComponentInChildren<TextMeshProUGUI>();
        restartText.text = "<color=#FFB90D>[RESTART]</color>";
    }

    public void IncreaseBulletBar(int startBar) {
        this._bulletsText.text = $"({GameManager.instance.pWeaponManager.GetCurrentMagazineCount()}/" +
                                 $"{GameManager.instance.pWeaponManager.GetMaxMagazineCount()})";
        for (var i = startBar; i < GameManager.instance.pWeaponManager.GetCurrentMagazineCount(); i++) {
            this._bulletBars[i].enabled = true;
        }
    }
    
    public void DecreaseBulletBar() {
        this._bulletsText.text = $"({GameManager.instance.pWeaponManager.GetCurrentMagazineCount()}/" +
                                 $"{GameManager.instance.pWeaponManager.GetMaxMagazineCount()})";
        this._bulletBars[GameManager.instance.pWeaponManager.GetCurrentMagazineCount()].enabled = false;
    }
    
    public void UpdateHealthText() {
        this._healthText.text = $"{GameManager.instance.player.GetCurrentHealth()}%";
    }
    
    public void DisplayFinishText() {
        if (LevelManager.CanAdvanceLevel() && !LevelManager.instance._isLevelEnded) this._topText.text = loadingNextLevel;
        else {
            Player.hasResetLevel = true;
            this._topText.text = "<color=#FF0000>*Translation Mismatch*</color>";
            this._topTextBGTransform.sizeDelta = new Vector2(this._failedTextBGWidth, this._topTextBGTransform.sizeDelta.y);
            this._restartButton.gameObject.SetActive(true);
        }
    }
    
    public void DisplayLevelText(float level) {
        //Adjust topText size based on the level text
        this._topTextBGTransform.sizeDelta = GameManager.instance.difficulty == GameManager.Difficulty.Normal 
            ? new Vector2(this._normalLevelTextBGWidth, this._topTextBGTransform.sizeDelta.y) 
            : new Vector2(this._baseLevelTextBGWidth, this._topTextBGTransform.sizeDelta.y);
        this._topText.text = $"{{Level {level}: {GameManager.instance.difficulty}}}";
    }

    public IEnumerator DisplayTimeToMemorize(float time) {
        //Adjust topText size based on the timeToMemorize text
        this._topTextBGTransform.sizeDelta = new Vector2(this._memorizeTextBGWidth, this._topTextBGTransform.sizeDelta.y);
        while (time > 0f) {
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topText.text = $"{{Memorize The Path: [{time:F2}s]}}";
            time -= Time.deltaTime;
            yield return null;
        }
        this._topText.text = $"{{Memorize The Path: [{0f:F2}s]}}";
    }
    
    public IEnumerator DisplayTimeToComplete() {
        //Adjust topText size based on the timeToComplete text
        this._topTextBGTransform.sizeDelta = new Vector2(this._completeTextBGWidth, this._topTextBGTransform.sizeDelta.y);
        while (!GameManager.instance.player.hasEnded && GameManager.instance.timeToComplete > 0f) {
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topText.text = $"{{Complete In: [{GameManager.instance.timeToComplete:F2}s]!}}";
            GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete - Time.deltaTime);
            yield return null;
        }
        this._topText.text = $"{{Complete In: [{0f:F2}s]!}}";
        // After timeToComplete is done or the submit button was clicked, set player.isEnded to true 
        GameManager.instance.player.hasEnded = true; 
    }
    
    public void SetButtons(bool b) {
        this._submitButton.interactable = b;
        this._resetButton.interactable = b;
        this._undoButton.interactable = b;
    }
}

