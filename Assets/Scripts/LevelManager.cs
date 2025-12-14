using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance;
    
    void Awake() {
        instance = this;
    }
    
    public IEnumerator StartLevel(AIManager aiManager) {
        GameManager.instance._aiPlayer.ResetSettings();
        GameManager.instance._player.ResetSettings();
        
        GameManager.instance.SetDifficulty(GameManager.instance.GetDifficulty);
        GridManager.instance.GenerateGrid();
        
        yield return StartCoroutine(aiManager.MoveSequence());
        // TODO: replace this with "WaitForSeconds(() => timerToComplete)"
        yield return new WaitUntil(() => GameManager.instance._player.isEnded);
        if (!GameManager.instance._aiPlayer.CompareMoves(GameManager.instance._player)) yield break;
        GameManager.instance.IncrementDifficulty();
        StartCoroutine(StartLevel(aiManager));
    }
}
