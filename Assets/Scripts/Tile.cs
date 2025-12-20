using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _offsetColor, _baseColor;
    [SerializeField] private Color _chestColor = Color.gold;
    [SerializeField] private Color _obstacleColor = Color.grey;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Stack<TileType> _tileTypesList;
    
    public bool isActivated { get; set; }

    public enum TileType {
        Regular, 
        Chest, 
        Obstacle
    }
    private TileType _tileType;
    public TileType GetTileType() => this._tileType;

    void Awake() {
        this._tileTypesList = new Stack<TileType>();
    }
    
    public void Init() {
        this._tileType = this._tileTypesList.Peek(); // Make last added TileType the current tileType of this instance
        HandleTileType();
    }
    
    private void HandleTileType() {
        switch (this._tileType) {
            case TileType.Regular: {
                var x = this.transform.position.x;
                var y = this.transform.position.y;
                var isOffset = (x % 2 == 0 && y  % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                this._spriteRenderer.color = isOffset ? this._offsetColor : this._baseColor;
                break;
            }
            case TileType.Chest: {
                this._spriteRenderer.color = this._chestColor;
                break;
            }
            case TileType.Obstacle: {
                this._spriteRenderer.color = this._obstacleColor;
                this.AddComponent<BoxCollider2D>();
                break;
            }
        }
    }

    public void AddToTileTypes(TileType tileType) {
        this._tileTypesList.Push(tileType);
        Init();
    }
    
    public void PopTileType() {
        this._tileTypesList.Pop();
        Init();
    }
    
    public TileType GetCurrentTileType() {
        return this._tileTypesList.Peek();
    }

    // For TileType.Obstacle
    // private void OnCollisionEnter2D(Collision2D other) {
    //     Debug.Log("Collision!");
    //     PopTileType();
    // }
}
