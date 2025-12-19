using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private Color _chestColor = Color.gold;
    [SerializeField] private Color _obstacleColor = Color.grey;
    [SerializeField] private Tile _tilePrefab; 
    [SerializeField] private Transform _cam;
    [SerializeField] private int _width, _height;
    
    public int Width => this._width;
    public int Height => this._height;
    
    public static GridManager instance;
    
    private readonly Dictionary<Vector2Int, Tile> _tiles = new();
    private readonly Dictionary<Vector2Int, Tile> _chestTiles = new();
    private readonly List<Tile> _obstacleTiles = new();
    
    private Tile _spawnedTile;

    void Awake() {
        instance = this;
    }
    
    public void GenerateGrid() {
        ClearGrid();
        for (var x = 0; x < this._width; x++) {
            for (var y = 0; y < this._height; y++) {
                this._spawnedTile = Instantiate(this._tilePrefab, new Vector3(x, y), Quaternion.identity);
                this._spawnedTile.Init(x, y);
                this._tiles.Add(new Vector2Int(x, y), this._spawnedTile);
            }
        }
        if (GameManager.instance.difficulty > GameManager.Difficulty.Easy) {
            MakeChestTiles(3);
        }
        // Positioning the cam position on the grid (based on the width and height)
        var centerWidth = (float) this._width / 2 + 1.25f;
        var centerHeight = (float) this._height / 2 - 0.5f;
        this._cam.position = new Vector3(centerWidth, centerHeight, -10); 
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MakeChestTiles(int number) {
        for (var i = 0; i < number; i++) {
            Vector2Int randomPos;

            // Generate a random tile position that is not in the chest tile already (no duplicates)
            // Uses the positions of the grid tiles
            do randomPos = new Vector2Int(Random.Range(1, this._width), Random.Range(0, this._height));
            while (this._chestTiles.ContainsValue(GetGridTileWithPosition(randomPos)));

            Tile chestTile = GetGridTileWithPosition(randomPos);

            chestTile.name = $"Chest Tile {randomPos.x}, {randomPos.y}";
            chestTile.GetComponent<SpriteRenderer>().color = this._chestColor;

            this._chestTiles.Add(new Vector2Int(randomPos.x, randomPos.y), chestTile);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void MakeObstacleTile(int number) {
        for (var i = 0; i < number; i++) {
            int randomIndex;

            // Generate a random tile position that is not in the obstacle tile already (no duplicates)
            // Uses the moves of the AI, not including the last one.
            do randomIndex = Random.Range(0, GameManager.instance.aiPlayer.GetMovesCount() - 1);
            while (this._obstacleTiles.Contains(GetGridTileWithPosition(GameManager.instance.aiPlayer.GetPosByIndex(randomIndex))));

            Debug.Log(randomIndex);
            Tile obstacleTile = GetGridTileWithPosition(GameManager.instance.aiPlayer.GetPosByIndex(randomIndex));
            
            if (!obstacleTile) break;

            obstacleTile.name =
                $"Obstacle Tile {obstacleTile.transform.position.x}, {obstacleTile.transform.position.y}";
            obstacleTile.GetComponent<SpriteRenderer>().color = this._obstacleColor;
            obstacleTile.AddComponent<BoxCollider2D>();
            
            this._obstacleTiles.Add(obstacleTile);
        }
    }

    public Tile GetChestTileWithPos(Vector2Int position) {
        return this._chestTiles.GetValueOrDefault(position);
    }

    private void ClearChestTiles() {
        foreach (Tile tile in this._chestTiles.Values) {
            tile.Init((int)tile.transform.position.x, (int)tile.transform.position.y);
        }

        this._chestTiles.Clear();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ClearObstacleTiles() {
        foreach (Tile tile in this._obstacleTiles) {
            tile.Init((int)tile.transform.position.x, (int)tile.transform.position.y);
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
    
    private Tile GetGridTileWithPosition(Vector2Int position) {
        return this._tiles.GetValueOrDefault(position);
    }
    
    public void SetWidth(int width) => this._width = width;
    public void SetHeight(int height) => this._height = height;
    
}
