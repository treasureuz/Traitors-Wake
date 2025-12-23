using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _timeToMove = 0.4f;
    private static readonly Vector2Int spawnPos = Vector2Int.zero;
    
    public void SetTimeToMove(float time) => this._timeToMove = time;
    public float TimeToMove => this._timeToMove;
    
    public static Vector3 SpawnPosV3() => new (spawnPos.x, spawnPos.y, 0);
    public static bool isMemorizing;
    public static bool hasResetLevel;
    
    public bool isMoving { get; private set; }
    public bool hasEnded { get; set; }
    
    [SerializeField] private List<Vector2Int> _moves;
    private List<Vector2Int> _directions;
    
    void Awake() {
        this._moves = new List<Vector2Int>();
        this._directions = new List<Vector2Int>();
        ResetSettings();
    }

    public IEnumerator MovePlayer(Vector2Int direction, float timeToMove) {
        if (this.hasEnded || this.isMoving) yield break;
        this.isMoving = true;
        
        Vector2Int startPos = new ((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = startPos + direction;
        
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.Height - 1 || GridManager.instance.GetGridTileWithPosition
                (targetPos).GetCurrentTileType() == Tile.TileType.Obstacle) {
            this.isMoving = false;
            yield break;
        } 
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(targetPos.x, targetPos.y);
        Vector2Int currentPos = new ((int) this.transform.position.x, (int) this.transform.position.y);
        
        this._moves.Add(currentPos); // Add current player position to the moves list
        this._directions.Add(direction); // Add the direction the player moved to the directions list
        this.isMoving = false;
        
        if (this != GameManager.instance.player) yield break;
        GridManager.instance.TryOpenChestTile(currentPos); // this calls ActivatePowerUp()
    }

    public bool MovesEquals(Player otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
    }
    
    public IEnumerator UndoMove(float timeToMove) {
        if (this._moves.Count == 0 || this.isMoving) yield break;
        this.isMoving = true;

        Vector2Int moveToRemove = this._directions[^1]; // ^1: gets the end index of the directions list
        Vector2Int startPos = new ((int) this.transform.position.x, (int) this.transform.position.y);
        Vector2Int targetPos = startPos - moveToRemove;
        
        this._moves.RemoveAt(this._moves.Count - 1); // Remove the last move from the list
        this._directions.RemoveAt(this._directions.Count - 1); // Remove the last direction from the list
        
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(targetPos.x, targetPos.y, 0);
        this.isMoving = false;
    }

    public IEnumerator ResetMoves(float timeToMove) {
        if (this._moves.Count == 0 || this.isMoving) yield break;
        this._moves.Clear();
        this.isMoving = true;
        
        Vector2Int startPos = new ((int) this.transform.position.x, (int) this.transform.position.y);
        Vector2Int targetPos = spawnPos;

        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(targetPos.x, targetPos.y);
        this.isMoving = false;
    }
    
    public void ResetSettings() {
        this._moves.Clear();
        this.transform.position = SpawnPosV3();
        this.hasEnded = false;
    }
    
    public int GetMovesCount() {
        return this._moves.Count;
    }

    public Vector2Int GetMovesPosByIndex(int index) {
        return this._moves[index];
    }
}
    
    
