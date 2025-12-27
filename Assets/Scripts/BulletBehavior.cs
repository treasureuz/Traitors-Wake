using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb2d;

    [Header("Global Bullet Settings")] 
    [SerializeField] private float _bulletRotationSpeed = 60f;
    [SerializeField] private float _bulletForce = 8f;
    [SerializeField] private float _destroyTime = 2f;

    void Start() { 
        LaunchBullet();
        DestroyAfter(this._destroyTime);
    }
    
    private void LaunchBullet() {
        // Both do the same thing
        //this._rb2d.AddForce(this.transform.right * this._bulletForce, ForceMode2D.Impulse);
        this._rb2d.linearVelocity = this.transform.right * this._bulletForce; // Better for this setting
        this._rb2d.angularVelocity = this._bulletRotationSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(this.gameObject); // Destroy on any collision (except the restricted ones in Edit -> Project Settings)
    }

    private void DestroyAfter(float time) {
        if (this.gameObject) Destroy(this.gameObject, time);
    }
}
