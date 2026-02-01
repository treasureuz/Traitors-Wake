using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _offsetColor, _baseColor;
    [SerializeField] private Color _baseObstacleColor = Color.grey, _offsetObstacleColor;
    [SerializeField] private Sprite _rockSprite1, _rockSprite2;
    [SerializeField] private Sprite _chestSprite;
    [SerializeField] private Sprite _finishLineSprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Stack<TileType> _tileTypesList;
    private SpriteRenderer _obstacleSR;

    private bool _isOffset;
    public bool isOpened { get; set; }
    
    // Can just utilize OOP by making ChestTile and ObstacleTile classes extending Tile (instead of this enum)
    public enum TileType {
        Regular, 
        Chest, 
        Obstacle
    }

    void Awake() {
        this._tileTypesList = new Stack<TileType>();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleTileType() {
        ColorTile(); // Colors this tile
        switch (this._tileTypesList.Peek()) { // Current tile type
            case TileType.Regular: {
                this.tag = "RegularTile"; break;
            } 
            case TileType.Chest: {
                // Assigns the unopened chestSprite and enables it
                this.transform.Find("Chest").GetComponent<SpriteRenderer>().sprite = this._chestSprite;
                this.transform.Find("Chest").gameObject.SetActive(true); 
                this.tag = "ChestTile";
                break;
            }
            case TileType.Obstacle: {
                // Enables the obstacle (rock) sprite
                this.transform.Find("Obstacle").gameObject.SetActive(true);
                SpriteRenderer tileSR = this.transform.Find("Obstacle").GetComponent<SpriteRenderer>();
                if (this._isOffset) {
                    // for the Asteroid sprite
                    tileSR.sprite = this._rockSprite2;
                    tileSR.transform.localScale = new Vector3(3, 3);
                } else {
                    // for the Rock sprite
                    tileSR.sprite = this._rockSprite1;
                    tileSR.transform.localScale = new Vector3(3.93f, 3.93f);
                }
                this.tag = "ObstacleTile";
                break;
            }
        }
    }

    public void AddToTileTypes(TileType tileType) {
        this._tileTypesList.Push(tileType); HandleTileType();
    }
    
    public void PopTileType() {
        this._tileTypesList.Pop(); HandleTileType();
    }
    
    public TileType GetCurrentTileType() {
        return this._tileTypesList.Peek();
    }

    private void ColorTile() {
        var x = Mathf.RoundToInt(this.transform.position.x); var y = Mathf.RoundToInt(this.transform.position.y);
        this._isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        switch (this._tileTypesList.Peek()) { // Current tile type
            case TileType.Regular:
            case TileType.Chest: 
                // Changes sprite to finishLineSprite if this tile is the finish position tile
                if (x == GridManager.instance.Width - 1 && y == GridManager.instance.Height - 1) {
                    this._spriteRenderer.sprite = this._finishLineSprite;
                    this._spriteRenderer.color = Color.white; return;
                }
                this._spriteRenderer.color = this._isOffset ? this._offsetColor : this._baseColor; break;
            case TileType.Obstacle: 
                this._spriteRenderer.color = this._isOffset ? this._offsetObstacleColor : this._baseObstacleColor;
                break;
        }
    }

    // For TileType.Obstacle
    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collision!");
        Vector2Int tilePos = new ((int) this.transform.position.x, (int) this.transform.position.y);
        GridManager.instance.RemoveObstacleTile(tilePos); // Removes the tile from obstacle tiles list
    }
}