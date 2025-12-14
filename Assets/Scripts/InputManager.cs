using System;
using System.Collections;
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
        if (this._player.isEnded || this._player.isMoving) return;
        if (this._up.IsPressed()) {
            StartCoroutine(this._player.MovePlayer(this._player, Vector3.up, this._player.PlayerTimeToMove));
        }
        if (this._down.IsPressed()) {
            StartCoroutine(this._player.MovePlayer(this._player, Vector3.down, this._player.PlayerTimeToMove));
        }
        if (this._left.IsPressed()) {
            StartCoroutine(this._player.MovePlayer(this._player, Vector3.left, this._player.PlayerTimeToMove));
        }
        if (this._right.IsPressed()) {
            StartCoroutine(this._player.MovePlayer(this._player, Vector3.right, this._player.PlayerTimeToMove));
        }
    }
}

