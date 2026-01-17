using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour {
    [SerializeField] private Transform _background;
    [SerializeField] private Transform _cam;
    [SerializeField] private SpriteRenderer _gridBorder;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject _tilesParent;
    [SerializeField] private Sprite _openedChestSprite;
    [SerializeField] private LayerMask _gridAreaMask;
    [SerializeField] private int _width, _height;
    
    public static GridManager instance;
    public SpriteRenderer[] tilesParentChildren { get; private set; }
    
    public int Width => this._width;
    public int Height => this._height;
    
    private readonly Dictionary<Vector2Int, Tile> _tiles = new();
    private readonly Dictionary<Vector2Int, Tile> _chestTiles = new();
    private readonly Dictionary<Vector2Int, Tile> _obstacleTiles = new();
    
    private Tile _spawnedTile;

    void Awake() {
        instance = this;
        this._gridBorder.enabled = false;
        this.tilesParentChildren = this._tilesParent.GetComponentsInChildren<SpriteRenderer>();
        // Deactivate obstacle and chest tile sprites
        this._tilePrefab.transform.Find("Obstacle").gameObject.SetActive(false);
        this._tilePrefab.transform.Find("Chest").gameObject.SetActive(false);
    }
    
    public void GenerateGrid() {
        ClearGrid();
        // Position the every relevant GameObject at the center of the grid (based on the width and height)
        var centerWidth = (float) this._width / 2 - 0.5f;
        var centerHeight = (float) this._height / 2 - 0.5f;
        this._cam.position = new Vector3(centerWidth, centerHeight, -10); 
        this._gridBorder.enabled = true;
        this.transform.localScale = new Vector3(this._width + 0.09f, this._height + 0.09f, 1); // creates a border
        this.transform.position = new Vector3(centerWidth, centerHeight);
        this._background.position = this.transform.position;
        
        for (var x = 0; x < this._width; x++) {
            for (var y = 0; y < this._height; y++) {
                this._spawnedTile = Instantiate(this._tilePrefab, new Vector3(x, y), Quaternion.identity, this._tilesParent.transform);
                this._spawnedTile.AddToTileTypes(Tile.TileType.Regular); // this calls HandleTileType()
                this._tiles.Add(new Vector2Int(x, y), this._spawnedTile);
            }
        }
        MakeChestTiles(GameManager.instance.numOfChests); // Make X amount of chest tiles
    }

    public bool IsWithinGridArea() {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return Physics2D.OverlapPoint(worldPos, _gridAreaMask);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void MakeChestTiles(int number) {
        for (var i = 0; i < number; i++) {
            Vector2Int randomPos;
            // Generate a random tile position that is not in the chest tile list already (no duplicates)
            // Uses the positions of the grid tiles
            do randomPos = new Vector2Int(Random.Range(1, this._width), Random.Range(1, this._height));
            while (this._chestTiles.ContainsValue(GetGridTileWithPos(randomPos)));
            
            Tile chestTile = GetGridTileWithPos(randomPos);
            // Setup chest tile
            chestTile.name = $"Chest Tile {randomPos.x}, {randomPos.y}";
            chestTile.AddToTileTypes(Tile.TileType.Chest); // this calls HandleTileType()
            this._chestTiles.Add(new Vector2Int(randomPos.x, randomPos.y), chestTile);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void TryOpenChestTile(Vector2Int position) {
        Tile chestTile = this._chestTiles.GetValueOrDefault(position);
        if (!chestTile || chestTile.isOpened) return;
        // If chestTile is not opened and the player's position is on it, open the chest and activate its PowerUp
        // Also changes the chestTile sprite to the opened version
        chestTile.isOpened = true; 
        chestTile.transform.Find("Chest").GetComponent<SpriteRenderer>().sprite = this._openedChestSprite;
        PowerUpManager.instance.ActivatePowerUp();
        chestTile.PopTileType(); // this calls HandleTileType();
        this._chestTiles.Remove(position);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void MakeObstacleTile(int number) {
        for (var i = 0; i < number; i++) {
            Vector2Int randomPos;
            // Generate a random tile position that is not in the obstacle tile list already (no duplicates)
            // Uses the moves of the AI, not including the last one.
            do {
                randomPos = GameManager.instance.difficulty == GameManager.Difficulty.Hard ? 
                    GameManager.instance.traitor.GetMovesPosByIndex
                        (Random.Range(0, GameManager.instance.traitor.GetMovesCount() - 1)) 
                    : new Vector2Int(Random.Range(1, this._width), Random.Range(1, this._height));
            }
            while (this._obstacleTiles.ContainsValue(GetGridTileWithPos(randomPos)));

            Tile obstacleTile = GetGridTileWithPos(randomPos);
            // Setup obstacle tile
            obstacleTile.name = $"Obstacle Tile {randomPos.x}, {randomPos.y}";
            obstacleTile.AddComponent<BoxCollider2D>();
            obstacleTile.AddToTileTypes(Tile.TileType.Obstacle); // this calls HandleTileType()
            this._obstacleTiles.Add(new Vector2Int(randomPos.x, randomPos.y), obstacleTile);
        }
    }
    
    public void RemoveObstacleTile(Vector2Int position) {
        Tile tileToRemove = this._obstacleTiles[position];
        // Disables the obstacle (rock) sprite
        tileToRemove.transform.Find("Obstacle").gameObject.SetActive(false);
        Destroy(tileToRemove.GetComponent<BoxCollider2D>());
        tileToRemove.PopTileType(); // Removes TileType.Obstacle
        this._obstacleTiles.Remove(position);
    }
    
    public void SetWidth(int width) => this._width = width;
    public void SetHeight(int height) => this._height = height;
    
    public Tile GetGridTileWithPos(Vector2Int position) {
        return this._tiles.GetValueOrDefault(position);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void ClearChestTiles() {
        foreach (Tile tile in this._chestTiles.Values) {
            tile.PopTileType(); // this calls HandleTileType()
            tile.transform.Find("Obstacle").gameObject.SetActive(false); // Disables the chest sprite
        }
        this._chestTiles.Clear();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ClearObstacleTiles() {
        foreach (Tile tile in this._obstacleTiles.Values) {
            tile.PopTileType(); // this calls HandleTileType()
            tile.transform.Find("Obstacle").gameObject.SetActive(false); // Disables the obstacle (rock) sprite
            Destroy(tile.GetComponent<BoxCollider2D>());
        }
        this._obstacleTiles.Clear();
    }

    public void ClearAllTileTypes() {
        ClearChestTiles();
        ClearObstacleTiles();
    }
    
    private void ClearGrid() {
        foreach (Tile tile in this._tiles.Values) Destroy(tile.gameObject);
        this._tiles.Clear();
    }
}
