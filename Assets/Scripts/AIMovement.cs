using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AIMovement : MonoBehaviour {
    [SerializeField] private Player _aiPlayer;
    [SerializeField] private Player _playerPrefab;
    
    private static readonly Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
    private Vector3 _randomDir;
    
    void Start() {
        this.transform.position = Vector3.zero;
        StartCoroutine(MoveSequence());
    }
    
    private IEnumerator MoveSequence() {
        int num;
        // Pick random direction that isn't left or down. *Only runs at the start*
        do num = UnityEngine.Random.Range(0, directions.Length);
        while (directions[num] == Vector3.left || directions[num] == Vector3.down);
        this._randomDir = directions[num];
        
        while (this.transform.position != new Vector3(GridManager.instance.Width - 1, GridManager.instance.Height - 1)) {
            yield return StartCoroutine(this._aiPlayer.MovePlayer(this._aiPlayer.transform, this._randomDir));
            
            Vector2Int gridPos = Vector2Int.RoundToInt(transform.position);

            // Build valid directions
            var validDirs = new List<Vector3>();
            
            if (gridPos.y < GridManager.instance.Height - 1) validDirs.Add(Vector3.up);
            if (gridPos.y > 0) validDirs.Add(Vector3.down);
            if (gridPos.x > 0) validDirs.Add(Vector3.left);
            if (gridPos.x < GridManager.instance.Width - 1) validDirs.Add(Vector3.right);

            // Remove reverse direction (e.g. if Vector3.up was the previous dir and is in the list, remove Vector3.down)
            validDirs.Remove(-this._randomDir);

            // Pick next valid direction
            this._randomDir = validDirs[UnityEngine.Random.Range(0, validDirs.Count)];
        } 
        // Spawn player when AI is positioned at finished pos: top-right.
        Instantiate(this._playerPrefab, this._playerPrefab.transform.position, Quaternion.identity);
    }
}
