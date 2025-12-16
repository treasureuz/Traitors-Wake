using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _offsetColor, _baseColor;
    [SerializeField] private SpriteRenderer _renderer;
    
    public void ColorTiles(int x, int y) {
        bool isOffset = (x % 2 == 0 && y  % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        _renderer.color = isOffset ? this._offsetColor : this._baseColor;
    }
    
    
}
