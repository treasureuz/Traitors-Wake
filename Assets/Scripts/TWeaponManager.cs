using UnityEngine;

public class TWeaponManager : WeaponManager {
    [SerializeField] private float minTimeBetweenShots = 3.5f;
    [SerializeField] private float maxTimeBetweenShots = 8f;
    [SerializeField] private float _bulletDamage = 13.5f;

    private float timeBetweenShots;

    void Awake() {
        this._owner = this.GetComponent<Traitor>();
    }
    
    protected override void Start() {
        base.Start(); // base = super
        // Set start timeBetweenShots
        timeBetweenShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    protected override Vector3 GetTargetPosition() {
        return GameManager.instance.player.transform.position;
    }

    protected override void HandleShoot() {
        if (!this._owner.hasEnded || GameManager.instance.player.hasEnded || LevelManager.instance._isLevelEnded 
            || !HasBullets()) return;
        this._nextShootTime += Time.deltaTime;
        if (this._nextShootTime < timeBetweenShots) return;
        Shoot();
        // Randomize new timeBetweenShots
        timeBetweenShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        this._nextShootTime = 0f;
    }

    public float GetBulletDamage() => this._bulletDamage;
}