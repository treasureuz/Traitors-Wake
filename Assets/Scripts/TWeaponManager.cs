using UnityEngine;

public class TWeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _minTimeBetweenShots = 3.5f;
    [SerializeField] private float _maxTimeBetweenShots = 8f;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private int _bulletMagazineCount = 10;

    private const float rotationDuration = 0.085f; //How long to rotate towards mouse position

    private GameObject _spawnedBullet;
    private Player _traitor;
    private float _nextShootTime;
    private int _currentMagazineCount;

    void Awake() {
        this._traitor = this.GetComponent<Player>();
    }
    
    void Start() {
        this._currentMagazineCount = this._bulletMagazineCount;
        // Set start timeBetweenShots
        this._timeBetweenShots = Random.Range(this._minTimeBetweenShots, this._maxTimeBetweenShots); 
    }

    void Update() {
        // Check if direction is positive (player position is to the right) or negative (player position is to the left)
        Vector3 direction = (GameManager.instance.player.transform.position - this.transform.position).normalized;
        var angle = Vector3.SignedAngle(this.transform.right, direction, Vector3.forward);
        // Rotate smoothly
        var t = Time.deltaTime / rotationDuration; // a fraction of the total angle/rotation (THIS frame)
        this.transform.Rotate(Vector3.forward, angle * t);
        HandleTraitorShoot();
    }

    private void HandleTraitorShoot() {
        if (!this._traitor.hasEnded || !HasBullets()) return;
        if (this._timeBetweenShots > this._nextShootTime) {
            this._nextShootTime += Time.deltaTime; return;
        }
        this._spawnedBullet = Instantiate(this._bulletPrefab, this._bulletSpawnPoint.position, this._bulletSpawnPoint.rotation);
        this._currentMagazineCount -= 1; // Decrement mag count
        // Randomize new timeBetweenShots
        this._timeBetweenShots = Random.Range(this._minTimeBetweenShots, this._maxTimeBetweenShots);
        this._nextShootTime = 0f; // Reset nextShootTime
    }
    
    private bool HasBullets() {
        return this._currentMagazineCount > 0;
    }
}
