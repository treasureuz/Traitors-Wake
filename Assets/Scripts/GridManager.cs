using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab; 
    [SerializeField] private Transform _cam;
    
    private Tile _spawnedTile;

    void Start() {
        GenerateGrid();
    }
    
    void GenerateGrid() {
        for (int i = 0; i < this._width; i++) {
            for (int j = 0; j < this._height; j++) {
                this._spawnedTile = Instantiate(this._tilePrefab, new Vector2(i, j), Quaternion.identity);
                
                bool isOffset = (i % 2 == 0 && j  % 2 != 0) || (i % 2 != 0 && j  % 2 == 0);
                this._spawnedTile.Init(isOffset);
            }
        }
        this._cam.position = new Vector3((float) this._width/2 - 0.5f, (float) this._height/2, -10);
    }
}
