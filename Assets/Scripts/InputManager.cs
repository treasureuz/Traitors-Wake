using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    [SerializeField] private Player _player;
    
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

    void Update() {
        if (this._up.IsPressed() && !this._player.IsMoving) {
            StartCoroutine(this._player.MovePlayer(this._player.transform, Vector3.up));
        }
        if (this._down.IsPressed() && !this._player.IsMoving) StartCoroutine(this._player.MovePlayer(this._player.transform, Vector3.down));
        if (this._left.IsPressed() && !this._player.IsMoving) StartCoroutine(this._player.MovePlayer(this._player.transform, Vector3.left));
        if (this._right.IsPressed() && !this._player.IsMoving) StartCoroutine(this._player.MovePlayer(this._player.transform, Vector3.right));
    }
}

