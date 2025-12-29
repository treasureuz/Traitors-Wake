using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _offsetColor, _baseColor;
    [SerializeField] private Color _chestColor = Color.gold;
    [SerializeField] private Color _baseObstacleColor = Color.grey, _offsetObstacleColor;
    [SerializeField] private Sprite _rockSprite1, _rockSprite2;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Stack<TileType> _tileTypesList;
    private SpriteRenderer[] _tilesSR;
    public bool isOpened { get; set; }

    public enum TileType {
        Regular, 
        Chest, 
        Obstacle
    }
    private TileType _tileType;
    public TileType GetTileType() => this._tileType;

    void Awake() {
        this._tileTypesList = new Stack<TileType>();
        this._tilesSR = this.GetComponentsInChildren<SpriteRenderer>(true);
    }
    
    public void Init() {
        this._tileType = this._tileTypesList.Peek(); // Make last added TileType the current tileType of this instance
        HandleTileType();
    }
    
    private void HandleTileType() {
        var x = this.transform.position.x; var y = this.transform.position.y;
        var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        switch (this._tileType) {
            case TileType.Regular: {
                this._spriteRenderer.color = isOffset ? this._offsetColor : this._baseColor;
                this.tag = "RegularTile";
                break;
            }
            case TileType.Chest: {
                this._spriteRenderer.color = this._chestColor;
                this.tag = "ChestTile";
                break;
            }
            case TileType.Obstacle: {
                if (isOffset) {
                    // for the Asteroid sprite
                    foreach (SpriteRenderer tileSR in this._tilesSR) { // Skips the parent gameObject
                        if (!tileSR.CompareTag("ObstacleTile")) continue;
                        tileSR.sprite = this._rockSprite2;
                        tileSR.transform.localScale = new Vector3(3, 3);
                    }
                    this._spriteRenderer.color = this._offsetObstacleColor;
                } else {
                    // for the Rock sprite
                    foreach (SpriteRenderer tileSR in this._tilesSR) { // Skips the parent gameObject
                        if (!tileSR.CompareTag("ObstacleTile")) continue;
                        tileSR.sprite = this._rockSprite1;
                        tileSR.transform.localScale = new Vector3(3.8f, 3.8f);
                        tileSR.transform.Rotate(0, 0, -10);
                    }
                    this._spriteRenderer.color = this._baseObstacleColor;
                }
                this.tag = "ObstacleTile";
                break;
            }
        }
    }

    public void AddToTileTypes(TileType tileType) {
        this._tileTypesList.Push(tileType); Init();
    }
    
    public void PopTileType() {
        this._tileTypesList.Pop(); Init();
    }
    
    public TileType GetCurrentTileType() {
        return this._tileTypesList.Peek();
    }

    // For TileType.Obstacle
    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collision!");
        PopTileType(); // Removes TileType.Obstacle
        Vector2Int tilePos = new ((int) this.transform.position.x, (int) this.transform.position.y);
        GridManager.instance.RemoveObstacleTile(tilePos); // Removes the tile from obstacle tiles list
    }
}
