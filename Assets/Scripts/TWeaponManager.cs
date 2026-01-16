using UnityEngine;

public class TWeaponManager : WeaponManager {
    [SerializeField] private float _minTimeBetweenShots = 3.5f;
    [SerializeField] private float _maxTimeBetweenShots = 8f;

    private float _timeBetweenShots;

    void Awake() {
        this._owner = this.GetComponentInParent<Traitor>();
    }
    
    void Start() {
        // Set start timeBetweenShots
        this._timeBetweenShots = Random.Range(this._minTimeBetweenShots, this._maxTimeBetweenShots);
    }

    protected override Vector3 GetTargetPosition() {
        return GameManager.instance.player.transform.position;
    }

    protected override void HandleShoot() {
        if (GameManager.isPaused || !this._owner.hasEnded || Player.isDead || GameManager.instance.player.hasEnded 
            || !HasBullets()) return;
        this._nextShootTime += Time.deltaTime;
        if (this._nextShootTime < this._timeBetweenShots) return;
        Shoot();
        // Randomize new timeBetweenShots
        this._timeBetweenShots = Random.Range(this._minTimeBetweenShots, this._maxTimeBetweenShots);
        this._nextShootTime = 0f;
    }

    public void SetMinTimeBetweenShots(float time) => this._minTimeBetweenShots = time;
    public void SetMaxTimeBetweenShots(float time) => this._maxTimeBetweenShots = time;
}