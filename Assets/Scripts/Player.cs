using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    [SerializeField] private float _playerTimeToMove = 0.5f;
    public static Vector2Int spawnPos = Vector2Int.zero;
    
    public float PlayerTimeToMove => this._playerTimeToMove;
    public static Vector3 SpawnPos() => new (spawnPos.x, spawnPos.y);
    public static bool isMemorizing;
    public static bool isReset;
    
    public bool isMoving { get; private set; }
    public bool isEnded { get; set; }
    
    [SerializeField] private List<Vector2Int> _moves;

    void Awake() {
        this._moves = new List<Vector2Int>();
        ResetSettings();
    }

    public IEnumerator MovePlayer(Vector2Int direction, float timeToMove) {
        if (this.isEnded || this.isMoving) yield break;
        this.isMoving = true;
        
        Vector2Int startPos = new ((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int targetPos = startPos + direction;
        
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0 
            || targetPos.y > GridManager.instance.Height - 1) {
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
        
        Vector2Int pos = new ((int)this.transform.position.x, (int)this.transform.position.y);
        this._moves.Add(pos);
        this.isMoving = false;
        
        if (this != GameManager.instance.player) yield break;
        
        Tile.TileType tileType = GridManager.instance.GetGridTileWithPosition(pos).GetCurrentTileType();
        switch (tileType) {
            // Check if current tile position is a TileType.Obstacle or a TileType.Chest
            case Tile.TileType.Obstacle:
                GridManager.instance.TryActivateObstacleTile(pos);
                break;
            case Tile.TileType.Chest:
                GridManager.instance.TryActivateChestTile(pos); // this calls ActivatePowerUp()
                break;
        }
    }

    public bool MovesEquals(Player otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
    }
    
    public IEnumerator UndoMove(float timeToMove) {
        if (this._moves.Count == 0 || this.isMoving) yield break;
        this.isMoving = true;

        Vector2Int startPos = new ((int)this.transform.position.x, (int)this.transform.position.y);
        Vector2Int moveToRemove = this._moves[^1]; // ^1: gets the end index of a list

        this._moves.RemoveAt(this._moves.Count - 1); // Remove the last move from the list

        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector2.Lerp(startPos, moveToRemove, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(moveToRemove.x, moveToRemove.y);
        this.isMoving = false;
    }

    public IEnumerator ResetMoves(float timeToMove) {
        if (this._moves.Count == 0 || this.isMoving) yield break;
        this._moves.Clear();
        this.isMoving = true;
        
        Vector2Int startPos = new ((int)this.transform.position.x, (int)this.transform.position.y);
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
        this.transform.position = SpawnPos();
        this.isEnded = false;
    }
    
    public int GetMovesCount() {
        return this._moves.Count;
    }

    public Vector2Int GetPosByIndex(int index) {
        return this._moves[index];
    }
}
    
    
