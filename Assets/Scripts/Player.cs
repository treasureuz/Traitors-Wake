using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerManager {
    [SerializeField] private List<Vector2Int> _directions;
    
    public static bool hasWon;
    
    private InputManager _inputManager;
    public float TimeToMove => this._timeToMove;

    protected override void Awake() {
        base.Awake();
        this._directions = new List<Vector2Int>();
        this._inputManager = this.GetComponent<InputManager>();
    }
    
    void Update() {
        if (!GameManager.instance.traitor.hasEnded || this.hasEnded || LevelManager.isGameEnded) return;
        if (this._inputManager.UpIsPressed()) StartCoroutine(HandleMovement(Vector2Int.up, this._timeToMove));
        else if (this._inputManager.DownIsPressed()) StartCoroutine(HandleMovement(Vector2Int.down, this._timeToMove));
        else if (this._inputManager.LeftIsPressed()) StartCoroutine(HandleMovement(Vector2Int.left, this._timeToMove));
        else if (this._inputManager.RightIsPressed()) StartCoroutine(HandleMovement(Vector2Int.right, this._timeToMove));
    }
    
    protected override IEnumerator HandleMovement(Vector2Int direction, float timeToMove) {
        if (this.hasEnded || this.isMoving) yield break;
        this.isMoving = true;
        
        Vector2Int startPos = new((int) this.transform.position.x, (int) this.transform.position.y);
        Vector2Int targetPos = startPos + direction;
        
        if (targetPos.x < 0 || targetPos.x > GridManager.instance.Width - 1 || targetPos.y < 0
            || targetPos.y > GridManager.instance.Height - 1 || GridManager.instance.
            GetGridTileWithPos(targetPos).GetCurrentTileType() == Tile.TileType.Obstacle) {
            this.isMoving = false; yield break;
        }
        
        var elapsedTime = 0f;
        while (elapsedTime < timeToMove) {
            this.transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(targetPos.x, targetPos.y);
        Vector2Int currentPos = new((int) this.transform.position.x, (int) this.transform.position.y);
        this._moves.Add(currentPos); // Add current player position to the moves list
        this._directions.Add(direction); // Add the direction the player moved to the directions list

        // Check if tile at currentPos after move is a TileType.ChestTile
        GridManager.instance.TryOpenChestTile(currentPos); // this calls ActivatePowerUp()
        this.isMoving = false;
    }
    
    public IEnumerator UndoMove(float timeToMove) {
        if (this._moves.Count == 0 || this.isMoving) yield break;
        this.isMoving = true;

        Vector2Int dirToRemove = this._directions[^1]; // ^1: gets the end index of the directions list
        Vector2Int startPos = new ((int) this.transform.position.x, (int) this.transform.position.y);
        Vector2Int targetPos = startPos - dirToRemove;
        
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
        this._directions.Clear();
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

    public override void ResetPlayerSettings() {
        this._currentHealth = this._maxHealth;
        UIManager.instance.UpdatePlayerHealthText();
        this._weaponManager.SetCurrentMagazineCount(this._weaponManager.GetMaxMagazineCount());
        UIManager.instance.UpdateBulletBar(true); // Enables all bullet bars
    }

    public override void ResetLevelSettings() {
        this._directions.Clear();
        base.ResetLevelSettings();
    }

    protected override void OnDamaged(float damageAmount) {
        base.OnDamaged(damageAmount);
        UIManager.instance.UpdatePlayerHealthText();
        if (this._currentHealth != 0) return;
        // If player is dead
        hasWon = false; this.gameObject.SetActive(false);
        LevelManager.instance.EndGame();
    }
    
    public void HealPlayer(int healAmount) {
        if (this._currentHealth >= this._maxHealth) return;
        this._currentHealth = Mathf.Clamp(this._currentHealth += healAmount, 0, this._maxHealth);
        UIManager.instance.UpdatePlayerHealthText();
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("TraitorBullet")) return;
        OnDamaged(GameManager.instance.tWeaponManager.GetBulletDamage());
    }
}
