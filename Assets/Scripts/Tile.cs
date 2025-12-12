using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _offsetColor, _baseColor;
    [SerializeField] private SpriteRenderer _renderer;
    
    public void Init(bool isOffset) {
        _renderer.color = isOffset ? this._offsetColor : this._baseColor;
    }
}
