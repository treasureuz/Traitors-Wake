using UnityEngine;
using UnityEngine.InputSystem;

public class PWeaponManager : MonoBehaviour {
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _timeBetweenShots = 3f;
    [SerializeField] private int _bulletMagazineCount = 10;

    private const float rotationDuration = 0.085f; //How long to rotate towards mouse position

    private GameObject _spawnedBullet;
    private Player _player;
    private float _nextShootTime;
    private int _currentMagazineCount;

    void Awake() {
        this._player = this.GetComponent<Player>();
    }
    
    void Start() {
        this._currentMagazineCount = this._bulletMagazineCount;
    }

    void Update() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // Check if direction is positive (mouse position is to the right) or negative (mouse position is to the left)
        Vector3 direction = (mousePosition - this.transform.position).normalized;
        var angle = Vector3.SignedAngle(this.transform.right, direction, Vector3.forward);
        // Rotate smoothly
        var t = Time.deltaTime / rotationDuration; // a fraction of the total angle/rotation (THIS frame)
        this.transform.Rotate(Vector3.forward, angle * t);
        HandlePlayerShoot();
    }

    private void HandlePlayerShoot() {
        if (!GameManager.instance.traitor.hasEnded || Player.hasResetLevel || this._player.hasEnded ||
            !Mouse.current.leftButton.isPressed || !HasBullets() || !(Time.time >= this._nextShootTime)) return;
        this._spawnedBullet = Instantiate(this._bulletPrefab, this._bulletSpawnPoint.position, this._bulletSpawnPoint.rotation);
        this._currentMagazineCount -= 1; // Decrement mag count
        UIManager.instance.DecreaseBulletBar();
        this._nextShootTime = Time.time + this._timeBetweenShots;
    }
    
    private bool HasBullets() {
        return this._currentMagazineCount > 0;
    }

    public void SetCurrentMagazineCount(int num) {
        if (this._currentMagazineCount + num > this._bulletMagazineCount) return;
        this._currentMagazineCount += num;
    }
    
    public int GetCurrentMagazineCount() => this._currentMagazineCount;
    public int GetMaxMagazineCount() => this._bulletMagazineCount;
}
