using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _submitButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _topTextTMP;
    [SerializeField] private RectTransform _actionButtons;

    [SerializeField] private float _baseLevelTextBGWidth = 480f;
    [SerializeField] private float _normalLevelTextBGWidth = 550f;
    [SerializeField] private float _memorizeTextBGWidth = 800f;
    [SerializeField] private float _completeTextBGWidth = 650f;
    [SerializeField] private float _failedTextBGWidth = 720f;
    
    public static UIManager instance;
    
    private RectTransform _topTextBGTransform;
    private const string loadingNextLevel = "Loading Next Level...";

    void Awake() {
        instance = this;
        this._topTextBGTransform = this._topTextTMP.GetComponentInParent<Image>().rectTransform;
        this._restartButton.gameObject.SetActive(false);
    }
    
    public void OnSubmit() {
        Debug.Log($"Submit: {GameManager.instance.player.isEnded}");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        
        GameManager.instance.player.isEnded = true;
        
        // Disable undo and reset after submission
        this._undoButton.interactable = false;
        this._resetButton.interactable = false;
    }

    public void OnUndo() {
        Debug.Log("Undo");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        StartCoroutine(GameManager.instance.player.UndoMove(GameManager.instance.player.PlayerTimeToMove));
    }
    
    public void OnReset() {
        Debug.Log("Reset");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        StartCoroutine(GameManager.instance.player.ResetMoves(GameManager.instance.player.PlayerTimeToMove));
    }
    
    public void OnRestart() {
        Debug.Log("Restart");
        EventSystem.current.SetSelectedGameObject(null); // removes "selectedButtonColor"
        this._restartButton.gameObject.SetActive(false);
        this._topTextTMP.text = loadingNextLevel;
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
    
    public bool CanAdvanceLevel() {
        bool rv;
        if (GameManager.instance.player.MovesEquals(GameManager.instance.aiPlayer)) {
            this._topTextTMP.text = loadingNextLevel;
            rv = true;
        } else {
            Player.isReset = true;
            this._topTextTMP.text = "<color=#FF0000>*Translation Mismatch*</color>";
            this._topTextBGTransform.sizeDelta = new Vector2(this._failedTextBGWidth, this._topTextBGTransform.sizeDelta.y);
            this._restartButton.gameObject.SetActive(true);
            rv = false;
        }
        return rv;
    }
    
    public void DisplayLevelText(float level) {
        //Adjust topText size based on the level text
        this._topTextBGTransform.sizeDelta = GameManager.instance.difficulty == GameManager.Difficulty.Normal 
            ? new Vector2(this._normalLevelTextBGWidth, this._topTextBGTransform.sizeDelta.y) 
            : new Vector2(this._baseLevelTextBGWidth, this._topTextBGTransform.sizeDelta.y);
        this._topTextTMP.text = $"{{Level {level}: {GameManager.instance.difficulty}}}";
    }

    public IEnumerator DisplayTimeToMemorize(float time) {
        //Adjust topText size based on the timeToMemorize text
        this._topTextBGTransform.sizeDelta = new Vector2(this._memorizeTextBGWidth, this._topTextBGTransform.sizeDelta.y);
        while (time > 0f) {
            Player.isMemorizing = true;
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topTextTMP.text = $"{{Memorize The Path: [{time:F2}s]}}";
            time -= Time.deltaTime;
            yield return null;
        }
        this._topTextTMP.text = $"{{Memorize The Path: [{0f:F2}s]}}";
    }
    
    public IEnumerator DisplayTimeToComplete() {
        //Adjust topText size based on the timeToComplete text
        this._topTextBGTransform.sizeDelta = new Vector2(this._completeTextBGWidth, this._topTextBGTransform.sizeDelta.y);
        while (!GameManager.instance.player.isEnded && GameManager.instance.timeToComplete > 0f) {
            // float.ToString("F2") or $"{float:F2}" converts the float value to 2 decimal places
            this._topTextTMP.text = $"{{Complete In: [{GameManager.instance.timeToComplete:F2}s]!}}";
            GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete - Time.deltaTime);
            yield return null;
        }
        this._topTextTMP.text = $"{{Complete In: [{0f:F2}s]!}}";
        // After timeToComplete is done or the submit button was clicked, set player.isEnded to true 
        GameManager.instance.player.isEnded = true; 
    }
    
    public void SetButtons(bool b) {
        this._submitButton.interactable = b;
        this._resetButton.interactable = b;
        this._undoButton.interactable = b;
    }
}

