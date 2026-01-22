using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Traitor : PlayersManager {
    [SerializeField] private LineRenderer _lineRenderer;
    
    private static readonly Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private static readonly List<Vector2Int> validDirs = new();

    private Coroutine _moveSequenceCoroutine;
    private Vector2Int _randomDir;
    private int _lineIndex;

    protected override void Awake() {
        base.Awake();
        this._lineRenderer.positionCount = 1;
    }
    
    private IEnumerator MoveSequence() {
        if (this.isMoving) yield break;
        this.isMoving = true;
        int num;
        // Pick random direction that isn't left or down.
        // **Only runs at the start**
        do num = Random.Range(0, directions.Length);
        while (directions[num] == Vector2Int.left || directions[num] == Vector2Int.down);
        this._randomDir = directions[num];

        this._lineIndex = 0;
        Vector3 finishPos = new (GameManager.instance.FinishPos.x, GameManager.instance.FinishPos.y);
        while (this.transform.position != finishPos) {
            if (GameManager.isPaused) yield return new WaitUntil(() => !GameManager.isPaused);
            StartCoroutine(LerpLineRenderer(this._timeToMove));
            yield return StartCoroutine(this.HandleMovement(this._randomDir, this._timeToMove));
            
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
        this.isMoving = false;
        if (GameManager.instance.GetPowerUpManagerByDiff().hasClearedObstacles) yield break;
        GridManager.instance.MakeObstacleTile(GameManager.instance.numOfObstacles);
    }

    private IEnumerator HandleMovement(Vector2Int direction, float timeToMove) {
        Vector2Int startPos = new((int) this.transform.position.x, (int) this.transform.position.y);
        Vector2Int targetPos = startPos + direction;
        
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove && !this.isDead) {
            this.transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(targetPos.x, targetPos.y);
        Vector2Int currentPos = new((int)this.transform.position.x, (int)this.transform.position.y);
        this._moves.Add(currentPos); // Add current player position to the moves list
    }

    private IEnumerator LerpLineRenderer(float timeToMove) {
        Vector2Int startPos = new ((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = startPos + this._randomDir;
        
        var elapsedTime = 0f;
        this._lineIndex++; this._lineRenderer.positionCount = this._lineIndex + 1;
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
        
        // Build weightedDirs by adding 'weight' amount of Vector3.up or Vector3.right to the list if they exist in validDirs
        var weightedDirs = new List<Vector2Int>();
        foreach (Vector2Int dir in validDirs)
            if (dir == Vector2Int.up || dir == Vector2Int.right) {
                for (var i = 0; i < weight; i++) weightedDirs.Add(dir);
            } else weightedDirs.Add(dir);
        
        // Pick next valid direction
        return weightedDirs[Random.Range(0, weightedDirs.Count)];
    }
    
    public void StartMoveSequenceCoroutine() {
        if (this._moveSequenceCoroutine != null) StopCoroutine(this._moveSequenceCoroutine);
        this._moveSequenceCoroutine = StartCoroutine(MoveSequence());
    }

    public void TryStopMoveSequenceCoroutine() {
        if (this._moveSequenceCoroutine == null) return;
        StopCoroutine(this._moveSequenceCoroutine);
        this._moveSequenceCoroutine = null;
        this.isMoving = false;
    }

    protected override void OnDamaged(float damageAmount) {
        base.OnDamaged(damageAmount);
        UIManager.instance.UpdateTraitorHealth();
        if (this._currentHealth != 0) return;
        InvokeOnDead(); // Invoke if traitor is killed
    }

    protected override void OnPlayerDead() {
        isDead = true; ScoreManager.instance.CalculateScores();
        LevelManager.instance.ActivateAllLevelsByDiff(); // Sets isCurrentEasy/Medium/HardCompleted to true
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("PlayerBullet")) return;
        OnDamaged(GameManager.instance.pWeaponManager.GetBulletDamage());
    }
    
    public void SetLRPosition(int val, Vector3 pos) => this._lineRenderer.SetPosition(val, pos);
    public void SetLRPosCount(int val) => this._lineRenderer.positionCount = val;
    public void SetLineRendererStatus(bool b) => this._lineRenderer.enabled = b;
}
