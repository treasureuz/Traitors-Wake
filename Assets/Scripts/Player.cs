using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _timeToMove = 0.3f;
    [SerializeField] private List<Vector3> _moves;
    public bool IsMoving { get; private set; }

    void Start() {
        this._moves = new List<Vector3>();
        this.transform.position = Vector3.zero;
    }

    public IEnumerator MovePlayer(Transform t, Vector3 direction) {
        if (this.IsMoving) yield break;
        this.IsMoving = true;
        
        Vector3 startPos = t.position;
        Vector3 targetPos = startPos + direction;
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.Height - 1) {
            targetPos = startPos;
        } 

        float elapsedTime = 0f;
        while (elapsedTime < this._timeToMove) {
            t.position = Vector3.Lerp(startPos, targetPos, elapsedTime / this._timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        t.position = targetPos;
        
        this._moves.Add(direction);
        this.IsMoving = false;
    }
}
    
    
