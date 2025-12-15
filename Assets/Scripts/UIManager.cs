using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _submitButton;

    public static UIManager instance;
    
    void Awake() {
        instance = this;
        SetButtons(false);
    }
    
    public void OnSubmit() {
        GameManager.instance._player.isEnded = true;
        EventSystem.current.SetSelectedGameObject(null);
        this._undoButton.interactable = false;
        this._resetButton.interactable = false;
        Debug.Log($"Submit: {GameManager.instance._player.isEnded}");
    }

    public void OnUndo() {
        Debug.Log("Undo");
        StartCoroutine(GameManager.instance._player.UndoMove(GameManager.instance._player.PlayerTimeToMove));
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    public void OnReset() {
        Debug.Log("Reset");
        GameManager.instance._player.ResetSettings();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetButtons(bool b) {
        this._submitButton.interactable = b;
        this._resetButton.interactable = b;
        this._undoButton.interactable = b;
    }
}

