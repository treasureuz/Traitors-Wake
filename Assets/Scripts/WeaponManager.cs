using UnityEngine;

public abstract class WeaponManager : MonoBehaviour {
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _bulletSpawnPoint;
    [SerializeField] protected int _bulletMagazineCount = 10;

    private const float rotationDuration = 0.072f; //How long to rotate towards mouse position
    
    protected PlayerManager _owner;
    protected int _currentMagazineCount;
    protected float _nextShootTime;

    protected virtual void Start() {
        this._currentMagazineCount = this._bulletMagazineCount;
    }

    protected virtual void Update() {
        RotateTowardsTarget();
        HandleShoot();
    }

    protected void Shoot() {
        Instantiate(this._bulletPrefab, this._bulletSpawnPoint.position, this._bulletSpawnPoint.rotation);
        this._currentMagazineCount--; // Decrement mag count
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void RotateTowardsTarget() {
        // Check if direction is positive (target position is to the right) or negative (target position is to the left)
        Vector2 direction = (GetTargetPosition() - this.transform.position).normalized;
        var angle = Vector3.SignedAngle(transform.right, direction, Vector3.forward);
        transform.Rotate(Vector3.forward, angle);
    }
    
    protected bool HasBullets() => this._currentMagazineCount > 0;

    protected abstract Vector3 GetTargetPosition();
    protected abstract void HandleShoot();

}
