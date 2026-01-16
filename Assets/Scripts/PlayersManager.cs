using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public abstract class PlayersManager : MonoBehaviour {
    [SerializeField] protected WeaponManager _weaponManager;
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected List<Vector2Int> _moves;

    protected static readonly Vector2Int spawnPos = Vector2Int.zero;
    public static Vector3 SpawnPosV3() => new(spawnPos.x, spawnPos.y, 0);

    public bool isMoving { get; protected set; }
    public bool hasEnded { get; set; }

    protected int _currentHealth;
    protected float _timeToMove;
    public void SetTimeToMove(float time) => this._timeToMove = time;

    protected virtual void Awake() {
        this._moves = new List<Vector2Int>();
    }

    private void OnDestroy() {
        PlayersDataManager.instance.SavePlayersData();
    }

    protected virtual void OnDamaged(float damageAmount) {
        var damage = Mathf.RoundToInt(damageAmount);
        this._currentHealth = Mathf.Clamp(this._currentHealth - damage, 0, this._maxHealth);
    }
    
    public bool MovesEquals(PlayersManager otherPlayers) {
        return this._moves.SequenceEqual(otherPlayers._moves);
    }

    public void ResetPlayerSettings() {
        this._currentHealth = this._maxHealth;
        this._weaponManager.SetCurrentMagazineCount(this._weaponManager.GetMaxMagazineCount());
    }
    
    public virtual void ResetLevelSettings() {
        this._moves.Clear();
        this.transform.position = SpawnPosV3();
        this.hasEnded = false;
    }
    
    public void SetCurrentHealth(int health) => this._currentHealth = health; 
    public int GetCurrentHealth() => this._currentHealth;
    public int GetMaxHealth() => this._maxHealth;
    
    public int GetMovesCount() => this._moves.Count;
    public Vector2Int GetMovesPosByIndex(int index) => this._moves[index];
    
     protected abstract void OnCollisionEnter2D(Collision2D collision);
}
    
    
