using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _timeToMove = 0.3f;
    public bool _isMoving { get; private set; }

    void Start() {
        this.transform.position = Vector3.zero;
    }

    public IEnumerator MovePlayer(Transform t, Vector3 direction) {
        this._isMoving = true;

        Vector3 startPos = t.position;
        Vector3 targetPos = startPos + direction;
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.GetWidth - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.GetHeight - 1) {
            targetPos = startPos;
        } 

        float elapsedTime = 0f;
        while (elapsedTime < this._timeToMove) {
            t.position = Vector3.Lerp(startPos, targetPos, elapsedTime / this._timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        t.position = targetPos;
        this._isMoving = false;
    }
}
    
    
