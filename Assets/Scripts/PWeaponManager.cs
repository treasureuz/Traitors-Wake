using UnityEngine;
using UnityEngine.InputSystem;

public class PWeaponManager : WeaponManager {
    [SerializeField] private float _timeBetweenShots = 3f;

    void Awake() {
        this._owner = this.GetComponentInParent<Player>();
    }
    
    protected override void HandleShoot() {
        if (GameManager.isPaused || !GameManager.instance.traitor.hasEnded || LevelManager.hasResetRun || 
            this._owner.hasEnded || !Mouse.current.leftButton.isPressed || !GridManager.instance.IsWithinGridArea() 
            || !HasBullets() || Time.time < this._nextShootTime) return;
        Shoot();
        UIManager.instance.UpdateBulletBar(false);
        this._nextShootTime = Time.time + this._timeBetweenShots;
    }

    protected override Vector3 GetTargetPosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
