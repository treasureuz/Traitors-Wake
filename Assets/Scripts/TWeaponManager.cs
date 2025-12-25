using UnityEngine;

public class TWeaponManager : WeaponManager {
    [SerializeField] private float minTimeBetweenShots = 3.5f;
    [SerializeField] private float maxTimeBetweenShots = 8f;

    private float timeBetweenShots;

    void Awake() {
        this._owner = this.GetComponent<Traitor>();
    }
    
    void Start() {
        // Set start timeBetweenShots
        timeBetweenShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    protected override Vector3 GetTargetPosition() {
        return GameManager.instance.player.transform.position;
    }

    protected override void HandleShoot() {
        if (!this._owner.hasEnded || GameManager.instance.player.hasEnded || LevelManager.isGameEnded 
            || !HasBullets()) return;
        this._nextShootTime += Time.deltaTime;
        if (this._nextShootTime < timeBetweenShots) return;
        Shoot();
        // Randomize new timeBetweenShots
        timeBetweenShots = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        this._nextShootTime = 0f;
    }
}