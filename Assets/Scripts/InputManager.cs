using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    private static PlayerInput playerInput;
    
    private InputAction _up;
    private InputAction _down;
    private InputAction _left;
    private InputAction _right;

    void Awake() {
        playerInput = this.GetComponent<PlayerInput>();
    }

    void Start() {
        this._up = playerInput.actions["Up"];
        this._down = playerInput.actions["Down"];
        this._left = playerInput.actions["Left"];
        this._right = playerInput.actions["Right"];
    }

    public bool UpIsPressed() => this._up.IsPressed();
    public bool DownIsPressed() => this._down.IsPressed();
    public bool LeftIsPressed() => this._left.IsPressed();
    public bool RightIsPressed() => this._right.IsPressed();
}

