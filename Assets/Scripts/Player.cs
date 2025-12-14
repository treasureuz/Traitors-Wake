using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _playerTimeToMove = 0.5f;
    [SerializeField] private List<Vector3> _moves;
    
    public float PlayerTimeToMove => this._playerTimeToMove;
    public List<Vector3> GetMoves() => this._moves;
    
    public float aiTimeToMove { get; set; }
    public bool isMoving { get; private set; }
    public bool isEnded { get; private set; }

    void Start() {
        this._moves = new List<Vector3>();
        this.transform.position = Vector3.zero;
    }

    public IEnumerator MovePlayer(Player player, Vector3 direction, float timeToMove) {
        if (this.isMoving) yield break;
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
        
        this._moves.Add(direction);
        this.isMoving = false;

        if (player.transform.position == new Vector3(GridManager.instance.Width - 1, GridManager.instance.Height - 1)) {
            player.isEnded = true;
        }
    }

    public void ClearMoves() {
        this._moves.Clear();
    }
}
    
    
