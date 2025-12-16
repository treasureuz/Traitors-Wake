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

    public static bool isMemorizing;
    public static bool hasReset;
    
    public bool isMoving { get; private set; }
    public bool isEnded { get; set; }
    
    [SerializeField] private List<Vector3> _moves;

    void Awake() {
        this._moves = new List<Vector3>();
        ResetSettings();
    }

    public IEnumerator MovePlayer(Vector3 direction, float timeToMove) {
        if (this.isEnded || this.isMoving) yield break;
        this.isMoving = true;
        
        Vector3 startPos = this.transform.position;
        Vector3 targetPos = startPos + direction;
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.Height - 1) {
            this.isMoving = false;
            yield break;
        } 
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = targetPos;
        
        this._moves.Add(direction);
        this.isMoving = false;
    }

    public bool MovesEquals(Player otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
    }

    public IEnumerator UndoMove(float timeToMove) {
        if (this._moves.Count == 0 || this.isMoving) yield break;
        this.isMoving = true;

        Vector3 moveToRemove = this._moves[^1];
        Vector3 startPos = this.transform.position;
        Vector3 targetPos = startPos - moveToRemove; // ^1: gets the end index of the list

        this._moves.Remove(moveToRemove); // Remove the last move from the list

        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = targetPos;
        this.isMoving = false;
    }
    
    public void ResetSettings() {
        this._moves.Clear();
        this.transform.position = spawnPos;
        this.isEnded = false;
    }
}
    
    
