using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _playerTimeToMove = 0.5f;
    [SerializeField] private Vector3 spawnPos =  Vector3.zero;
    [SerializeField] private Stack<Vector3> _moves;
    
    public float PlayerTimeToMove => this._playerTimeToMove;
    public Vector3 SpawnPos => spawnPos;
    
    public float aiTimeToMove { get; set; }
    public bool isMoving { get; private set; }
    public bool isEnded { get; set; }

    void Awake() {
        this._moves = new Stack<Vector3>();
        ResetSettings();
    }

    public IEnumerator MovePlayer(Player player, Vector3 direction, float timeToMove) {
        if (this.isEnded || this.isMoving) yield break;
        this.isMoving = true;
        
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = startPos + direction;
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.Height - 1) {
            targetPos = startPos;
            this.isMoving = false;
            yield return null;
        } 
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            player.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        player.transform.position = targetPos;
        
        this._moves.Push(direction);
        this.isMoving = false;
    }

    public bool CompareMoves(Player otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
    }

    public void UndoMove() {
        if (this._moves.Count == 0) return;
        this.transform.position -= this._moves.Pop();
    }
    
    public void ResetSettings() {
        this._moves.Clear();
        this.transform.position = spawnPos;
        this.isEnded = false;
    }
}
    
    
