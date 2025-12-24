using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public abstract class PlayerManager : MonoBehaviour {
    [SerializeField] protected List<Vector2Int> _moves;
    
    protected static readonly Vector2Int spawnPos = Vector2Int.zero;
    public static Vector3 SpawnPosV3() => new (spawnPos.x, spawnPos.y, 0);
    
    public bool isMoving { get; protected set; }
    public bool hasEnded { get; set; }
    
    protected virtual void Awake() {
        this._moves = new List<Vector2Int>();
        ResetSettings();
    }

    protected virtual IEnumerator HandleMovement(Vector2Int direction, float timeToMove) {
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
        this.isMoving = false;
    }

    public bool MovesEquals(PlayerManager otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
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
    
    
