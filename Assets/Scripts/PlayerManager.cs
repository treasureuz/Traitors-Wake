using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public abstract class PlayerManager : MonoBehaviour {
    [SerializeField] protected WeaponManager _weaponManager;
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected List<Vector2Int> _moves;

    protected static readonly Vector2Int spawnPos = Vector2Int.zero;
    public static Vector3 SpawnPosV3() => new(spawnPos.x, spawnPos.y, 0);

    protected bool isMoving;
    public bool hasEnded { get; set; }

    protected int _currentHealth;

    protected virtual void Awake() {
        this._moves = new List<Vector2Int>();
    }

    protected virtual void Start() {
        ResetLevelSettings();
        ResetPlayerSettings(); // Sets currentHealth = maxHealth
    }

    protected virtual IEnumerator HandleMovement(Vector2Int direction, float timeToMove) {
        if (this.hasEnded || this.isMoving) yield break;
        this.isMoving = true;

        Vector2Int startPos = new((int)this.transform.position.x, (int)this.transform.position.y);
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
        Vector2Int currentPos = new((int)this.transform.position.x, (int)this.transform.position.y);
        this._moves.Add(currentPos); // Add current player position to the moves list
        this.isMoving = false;
    }

    protected virtual void OnDamaged(float damageAmount) {
        var damage = Mathf.RoundToInt(damageAmount);
        this._currentHealth = Mathf.Clamp(this._currentHealth - damage, 0, this._maxHealth);
    }

    public bool MovesEquals(PlayerManager otherPlayer) {
        return this._moves.SequenceEqual(otherPlayer._moves);
    }

    public abstract void ResetPlayerSettings();
    
    public void ResetLevelSettings() {
        this._moves.Clear();
        this.transform.position = SpawnPosV3();
        this.hasEnded = false;
    }
    
    public int GetCurrentHealth() => this._currentHealth;
    public int GetMaxHealth() => this._maxHealth;
    
    public int GetMovesCount() => this._moves.Count;
    public Vector2Int GetMovesPosByIndex(int index) => this._moves[index];
    
     protected abstract void OnCollisionEnter2D(Collision2D collision);
}
    
    
