using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AIManager : MonoBehaviour {
    [SerializeField] private Player _aiPlayer;
    [SerializeField] private Player _playerPrefab;
    
    private static readonly Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
    private static readonly List<Vector3> validDirs = new();
    
    private Vector3 _randomDir;
    
    void Start() {
        StartCoroutine(MoveSequence());
    }
    
    public IEnumerator MoveSequence() {
        int num;
        // Pick random direction that isn't left or down.
        // **Only runs at the start**
        do num = UnityEngine.Random.Range(0, directions.Length);
        while (directions[num] == Vector3.left || directions[num] == Vector3.down);
        this._randomDir = directions[num];
        
        while (this.transform.position != new Vector3(GridManager.instance.Width - 1, GridManager.instance.Height - 1)) {
            yield return StartCoroutine(this._aiPlayer.MovePlayer
                (this._aiPlayer, this._randomDir, this._aiPlayer.aiTimeToMove));
            Vector2Int gridPos = Vector2Int.RoundToInt(transform.position);
            
            validDirs.Clear();
            
            // Build valid directions
            if (gridPos.y < GridManager.instance.Height - 1) validDirs.Add(Vector3.up);
            if (gridPos.y > 0) validDirs.Add(Vector3.down);
            if (gridPos.x > 0) validDirs.Add(Vector3.left);
            if (gridPos.x < GridManager.instance.Width - 1) validDirs.Add(Vector3.right);

            // Remove reverse direction (e.g. if Vector3.up was the previous dir and is in the list, remove Vector3.down)
            validDirs.Remove(-this._randomDir);
            
            this._randomDir = NextDir();
        } 
    }

    private static Vector3 NextDir() {
        // Weight = numOfDirs that will get added the weightedDirs based on the current difficulty
        // **Works same as regular switch-case expression*
        var weight = GameManager.instance.numOfDirs;
        
        // Build weightedDirs by adding 'weight' amount of Vector3.up or Vector3.right to the list if they exist
        var weightedDirs = new List<Vector3>();
        foreach (Vector3 dir in validDirs)
            if (dir == Vector3.up || dir == Vector3.right) {
                for (var i = 0; i < weight; i++) weightedDirs.Add(dir);
            } else weightedDirs.Add(dir);
        
        // Pick next valid direction
        return weightedDirs[UnityEngine.Random.Range(0, weightedDirs.Count)];
    }
}
