using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void OnSubmit() {
        if (GameManager.instance._player.transform.position != GameManager.instance.FinishPos) return;
        GameManager.instance._player.isEnded = true;
        Debug.Log($"Sumbmit: {GameManager.instance._player.isEnded}");
    }

    public void OnUndo() {
        Debug.Log("Undo");
        GameManager.instance._player.UndoMove();
    }
    
    public void OnReset() {
        Debug.Log("Reset");
        GameManager.instance._player.ResetSettings();
    }
}
