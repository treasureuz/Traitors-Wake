using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TraitorManager : MonoBehaviour {
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private LineRenderer _lineRenderer;
    
    private static readonly Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private static readonly List<Vector2Int> validDirs = new();
    
    private Player _traitor;
    private Vector2Int _randomDir;
    private int _lineIndex;

    void Awake() {
        this._traitor = this.GetComponent<Player>();
        this._lineRenderer.positionCount = 1;
    }
    
    public IEnumerator MoveSequence() {
        int num;
        // Pick random direction that isn't left or down.
        // **Only runs at the start**
        do num = Random.Range(0, directions.Length);
        while (directions[num] == Vector2Int.left || directions[num] == Vector2Int.down);
        this._randomDir = directions[num];

        this._lineIndex = 0;
        Vector3 finishPos = new (GameManager.instance.FinishPos.x, GameManager.instance.FinishPos.y);
        while (this.transform.position != finishPos) {
            StartCoroutine(LerpLineRenderer(this._traitor.TimeToMove));
            yield return StartCoroutine(this._traitor.MovePlayer(this._randomDir, this._traitor.TimeToMove));
            
            Vector2Int gridPos = Vector2Int.RoundToInt(transform.position);
            
            validDirs.Clear();
            // Build valid directions
            if (gridPos.y < GridManager.instance.Height - 1) validDirs.Add(Vector2Int.up);
            if (gridPos.y > 0) validDirs.Add(Vector2Int.down);
            if (gridPos.x > 0) validDirs.Add(Vector2Int.left);
            if (gridPos.x < GridManager.instance.Width - 1) validDirs.Add(Vector2Int.right);

            // Remove reverse direction (e.g. if Vector3.up was the previous dir and is in the list, remove Vector3.down)
            validDirs.Remove(-this._randomDir);
            this._randomDir = NextDir();
        }
        GridManager.instance.MakeObstacleTile(7);
    }

    private IEnumerator LerpLineRenderer(float timeToMove) {
        Vector2Int startPos = new ((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = startPos + this._randomDir;
        
        var elapsedTime = 0f;
        this._lineIndex++;
        this._lineRenderer.positionCount = this._lineIndex + 1;
        while (elapsedTime < timeToMove) {
            this._lineRenderer.SetPosition(this._lineRenderer.positionCount - 1, 
                Vector2.Lerp(startPos, targetPos, elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this._lineRenderer.SetPosition(this._lineRenderer.positionCount - 1, new Vector3(targetPos.x, targetPos.y));
    }
    
    private static Vector2Int NextDir() {
        var weight = GameManager.instance.numOfDirs;
        
        // Build weightedDirs by adding 'weight' amount of Vector3.up or Vector3.right to the list if they exist
        var weightedDirs = new List<Vector2Int>();
        foreach (Vector2Int dir in validDirs)
            if (dir == Vector2Int.up || dir == Vector2Int.right) {
                for (var i = 0; i < weight; i++) weightedDirs.Add(dir);
            } else weightedDirs.Add(dir);
        
        // Pick next valid direction
        return weightedDirs[Random.Range(0, weightedDirs.Count)];
    }

    public void SetLRPosition(int val, Vector3 pos) => this._lineRenderer.SetPosition(val, pos);
    public void SetLRPosCount(int val) => this._lineRenderer.positionCount = val;
    public void SetLineRendererStatus(bool b) => this._lineRenderer.enabled = b;
}
