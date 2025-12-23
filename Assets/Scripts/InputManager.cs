using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    private static PlayerInput playerInput;
    private Player _player;
        
    private InputAction _up;
    private InputAction _down;
    private InputAction _left;
    private InputAction _right;

    void Awake() {
        playerInput = this.GetComponent<PlayerInput>();
        this._player = this.GetComponent<Player>();
    }

    void Start() {
        this._up = playerInput.actions["Up"];
        this._down = playerInput.actions["Down"];
        this._left = playerInput.actions["Left"];
        this._right = playerInput.actions["Right"];
    }

    void Update() {
        if (!GameManager.instance.traitor.hasEnded || this._player.hasEnded) return;
        if (this._up.IsPressed()) StartCoroutine(this._player.MovePlayer(Vector2Int.up, this._player.TimeToMove));
        if (this._down.IsPressed()) StartCoroutine(this._player.MovePlayer(Vector2Int.down, this._player.TimeToMove));
        if (this._left.IsPressed()) StartCoroutine(this._player.MovePlayer(Vector2Int.left, this._player.TimeToMove));
        if (this._right.IsPressed()) StartCoroutine(this._player.MovePlayer(Vector2Int.right, this._player.TimeToMove));
    }
}

