using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _playerTimeToMove = 0.5f;
    [SerializeField] private Vector3 spawnPos =  Vector3.zero;
    
    public float PlayerTimeToMove => this._playerTimeToMove;
    public Vector3 SpawnPos => spawnPos;

    private bool _isMoving;
    public bool isEnded { get; set; }
    
    private Stack<Vector3> _moves;

    void Awake() {
        this._moves = new Stack<Vector3>();
        ResetSettings();
    }

    public IEnumerator MovePlayer(Vector3 direction, float timeToMove) {
        if (this.isEnded || this._isMoving) yield break;
        this._isMoving = true;
        
        Vector3 startPos = this.transform.position;
        Vector3 targetPos = startPos + direction;
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.Height - 1) {
            targetPos = startPos;
            this._isMoving = false;
            yield return null;
        } 
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = targetPos;
        
        this._moves.Push(direction);
        this._isMoving = false;
    }

    public bool CompareMoves(Player otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
    }

    public IEnumerator UndoMove(float timeToMove) {
        if (this._moves.Count == 0 || this._isMoving) yield break;
        this._isMoving = true;
        
        Vector3 startPos = this.transform.position;
        Vector3 targetPos = startPos - this._moves.Pop();
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = targetPos;
        this._isMoving = false;
    }
    
    public void ResetSettings() {
        this._moves.Clear();
        this.transform.position = spawnPos;
        this.isEnded = false;
    }
}
    
    
