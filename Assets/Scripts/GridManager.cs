using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private Tile _tilePrefab; 
    [SerializeField] private Transform _cam;
    [SerializeField] private int _width, _height;
    
    public int Width => this._width;
    public int Height => this._height;
    
    public static GridManager instance;
    
    private readonly Dictionary<Vector2Int, Tile> _tiles = new();
    private readonly Dictionary<Vector2Int, Tile> _chestTiles = new();
    private readonly Dictionary<Vector2Int, Tile> _obstacleTiles = new();
    
    private Tile _spawnedTile;

    void Awake() {
        instance = this;
    }
    
    public void GenerateGrid() {
        ClearGrid();
        for (var x = 0; x < this._width; x++) {
            for (var y = 0; y < this._height; y++) {
                this._spawnedTile = Instantiate(this._tilePrefab, new Vector3(x, y), Quaternion.identity);
                this._spawnedTile.AddToTileTypes(Tile.TileType.Regular); // this calls Init()
                this._tiles.Add(new Vector2Int(x, y), this._spawnedTile);
            }
        }
        
        MakeChestTiles(6);
        
        // Position the cam position at the center of the grid (based on the width and height)
        var centerWidth = (float) this._width / 2 - 0.5f;
        var centerHeight = (float) this._height / 2 - 0.5f;
        this._cam.position = new Vector3(centerWidth, centerHeight, -10); 
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MakeChestTiles(int number) {
        for (var i = 0; i < number; i++) {
            Vector2Int randomPos;

            // Generate a random tile position that is not in the chest tile already (no duplicates)
            // Uses the positions of the grid tiles
            do randomPos = new Vector2Int(Random.Range(1, this._width), Random.Range(1, this._height));
            while (this._chestTiles.ContainsValue(GetGridTileWithPosition(randomPos)));

            Tile chestTile = GetGridTileWithPosition(randomPos);
            chestTile.AddToTileTypes(Tile.TileType.Chest); // this calls Init()
            chestTile.name = $"Chest Tile {randomPos.x}, {randomPos.y}";
            
            this._chestTiles.Add(new Vector2Int(randomPos.x, randomPos.y), chestTile);
        }
    }
    
    public void TryActivateChestTile(Vector2Int position) {
        Tile chestTile = this._chestTiles.GetValueOrDefault(position);
        if (!chestTile || chestTile.isActivated) return;
        chestTile.isActivated = true;
        PowerUpManager.instance.ActivatePowerUp();
        chestTile.PopTileType(); // this calls Init();
        this._chestTiles.Remove(position);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void MakeObstacleTile(int number) {
        for (var i = 0; i < number; i++) {
            Vector2Int randomPos;

            // Generate a random tile position that is not in the obstacle tile already (no duplicates)
            // Uses the moves of the AI, not including the last one.
            do {
                randomPos = GameManager.instance.difficulty == GameManager.Difficulty.Hard ?
                    GameManager.instance.aiPlayer.GetMovesPosByIndex(Random.Range(0, GameManager.instance.aiPlayer.GetMovesCount() - 1)) 
                    : new Vector2Int(Random.Range(1, this._width), Random.Range(1, this._height));
            }
            while (this._obstacleTiles.ContainsValue(GetGridTileWithPosition(randomPos)));

            Tile obstacleTile = GetGridTileWithPosition(randomPos);
            obstacleTile.AddToTileTypes(Tile.TileType.Obstacle); // this calls Init()
            obstacleTile.name = $"Obstacle Tile {randomPos.x}, {randomPos.y}";
            
            this._obstacleTiles.Add(new Vector2Int(randomPos.x, randomPos.y), obstacleTile);
        }
    }

    // TODO: Replace for "OnCollisionEnter2D"
    public void TryActivateObstacleTile(Vector2Int position) {
        Tile obstacleTile = this._obstacleTiles.GetValueOrDefault(position);
        if (!obstacleTile) return;
        obstacleTile.PopTileType(); // this calls Init();
        this._obstacleTiles.Remove(position);
    }
    
    // public bool IsObstacleTile(Vector2Int pos) {
    //     return this._obstacleTiles.ContainsKey(pos);
    // }
    
    private void ClearChestTiles() {
        foreach (Tile tile in this._chestTiles.Values) tile.PopTileType(); // this calls Init()
        this._chestTiles.Clear();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ClearObstacleTiles() {
        foreach (Tile tile in this._obstacleTiles.Values) {
            tile.PopTileType(); // this calls Init()
            Destroy(tile.GetComponent<BoxCollider2D>());
        }
        this._obstacleTiles.Clear();
    }

    public void ClearAllTiles() {
        ClearChestTiles();
        ClearObstacleTiles();
    }
    
    private void ClearGrid() {
        foreach (Tile tile in this._tiles.Values) Destroy(tile.gameObject);
        this._tiles.Clear();
    }
    
    public Tile GetGridTileWithPosition(Vector2Int position) {
        return this._tiles.GetValueOrDefault(position);
    }
    
    public void SetWidth(int width) => this._width = width;
    public void SetHeight(int height) => this._height = height;
    
}
