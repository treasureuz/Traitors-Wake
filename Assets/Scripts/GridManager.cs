using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private Tile _tilePrefab; 
    [SerializeField] private Transform _cam;
    [SerializeField] private int _width, _height;
    
    public int Width => this._width;
    public int Height => this._height;
    
    public static GridManager instance;
    
    private readonly Dictionary<Vector2, Tile> _tiles = new();
    private readonly List<Tile> _chestTiles = new();
    private Tile _spawnedTile;

    void Awake() {
        instance = this;
    }
    
    public void GenerateGrid() {
        ClearGrid();
        for (var x = 0; x < this._width; x++) {
            for (var y = 0; y < this._height; y++) {
                this._spawnedTile = Instantiate(this._tilePrefab, new Vector3(x, y), Quaternion.identity);
                this._spawnedTile.ColorTile(x, y);
                this._tiles.Add(new Vector2(x, y), this._spawnedTile);
            }
        }
        if (GameManager.instance.difficulty > GameManager.Difficulty.Easy) {
            MakeChestTiles(3);
        }
        // Positioning the cam position on the grid (based on the width and height)
        var centerWidth = (float)this._width / 2 - 2f;
        var centerHeight = (float) this._height / 2 - 0.5f;
        this._cam.position = new Vector3(centerWidth, centerHeight, -10); 
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MakeChestTiles(int number) {
        var positions = this._tiles.Keys.ToList();
        for (int i = 0; i < number; i++) {
            var num = Random.Range(1, this._tiles.Count);
            Tile tile = this._tiles[positions[num]];
            tile.GetComponent<SpriteRenderer>().color = Color.gold;
            this._chestTiles.Add(tile);
        }
    }

    public void ClearChestTiles() {
        this._chestTiles.Clear();
    }
    
    private void ClearGrid() {
        foreach (Tile tile in this._tiles.Values) Destroy(tile.gameObject);
        this._tiles.Clear();
    }

    public void SetWidth(int width) {
        this._width = width;
    }

    public void SetHeight(int height) {
        this._height = height;
    }
    
    public List<Tile> GetChestTiles() {
        return new List<Tile>(this._chestTiles);
    }
}
