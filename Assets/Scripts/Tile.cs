using System;
using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _offsetColor, _baseColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public bool isActivated { get; set; }
    
    public void ColorTile(int x, int y) {
        var isOffset = (x % 2 == 0 && y  % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        this._spriteRenderer.color = isOffset ? this._offsetColor : this._baseColor;
    }
}
