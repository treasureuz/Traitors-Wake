using UnityEngine;
using UnityEngine.InputSystem;

public class PWeaponManager : WeaponManager {
    [SerializeField] private float _timeBetweenShots = 3f;

    void Awake() {
        this._owner = this.GetComponent<Player>();
    }
    
    protected override void HandleShoot() {
        if (!GameManager.instance.traitor.hasEnded || Player.hasResetLevel || LevelManager.instance._isLevelEnded || 
            this._owner.hasEnded || !Mouse.current.leftButton.isPressed || !GridManager.instance.IsWithinGridArea() || 
            !HasBullets() || Time.time < this._nextShootTime) return;
        Shoot();
        UIManager.instance.DecreaseBulletBar();
        this._nextShootTime = Time.time + this._timeBetweenShots;
    }

    protected override Vector3 GetTargetPosition() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    
    public void SetCurrentMagazineCount(int num) {
        if (this._currentMagazineCount + num > this._bulletMagazineCount) return;
        this._currentMagazineCount += num;
    }
    
    public int GetCurrentMagazineCount() => this._currentMagazineCount;
    public int GetMaxMagazineCount() => this._bulletMagazineCount;
}
