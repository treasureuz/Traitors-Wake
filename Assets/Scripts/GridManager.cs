using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private Tile _tilePrefab; 
    [SerializeField] private Transform _cam;
    [SerializeField] private int _width, _height;
    
    public int Width => this._width;
    public int Height => this._height;
    
    public static GridManager instance;
    
    private Dictionary<Vector2, Tile> _tiles;
    private Tile _spawnedTile;

    void Awake() {
        instance = this;
        this._tiles = new Dictionary<Vector2, Tile>();
    }
    
    public void GenerateGrid() {
        ClearGrid();
        for (var x = 0; x < this._width; x++) {
            for (var y = 0; y < this._height; y++) {
                this._spawnedTile = Instantiate(this._tilePrefab, new Vector3(x, y), Quaternion.identity);
                this._spawnedTile.Init(x, y);
                this._tiles.Add(new Vector2(x, y), this._spawnedTile);
            }
        }
        // Centering the cam position on the grid (based on the width and height)
        this._cam.position = new Vector3((float) this._width/2 - 0.5f, (float) this._height/2 - 0.5f, -10); 
    }
    
    public Tile GetTileAtPosition(Vector2 position) {
        return this._tiles.GetValueOrDefault(position);
    }

    private void ClearGrid() {
        this._tiles.Clear();
    }

    public void SetWidth(int width) {
        this._width = width;
    }

    public void SetHeight(int height) {
        this._height = height;
    }
}
