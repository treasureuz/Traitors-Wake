using UnityEngine;

public abstract class WeaponManager : MonoBehaviour {
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _bulletSpawnPoint;
    [SerializeField] protected int _bulletMagazineCount = 10;
    [SerializeField] protected float _bulletDamage = 13.5f;

    private const float rotationDuration = 0.072f; //How long to rotate towards mouse position
    
    protected PlayerManager _owner;
    private int _currentMagazineCount;
    protected float _nextShootTime;

    protected virtual void Update() {
        RotateTowardsTarget();
        HandleShoot();
    }

    protected abstract void HandleShoot();
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
    protected abstract Vector3 GetTargetPosition();
    
    protected bool HasBullets() => this._currentMagazineCount > 0;
    public void SetCurrentMagazineCount(int num) => this._currentMagazineCount = num;
    public int GetCurrentMagazineCount() => this._currentMagazineCount;
    public int GetMaxMagazineCount() => this._bulletMagazineCount;
    public float GetBulletDamage() => this._bulletDamage;
    
}
