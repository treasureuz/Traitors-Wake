using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HomeSpritesBehavior : MonoBehaviour {
    [SerializeField] private GameObject [] _spritePrefabs;
    [SerializeField] private float _rotationSpeed = -75f;
    [SerializeField] private float _launchSpeed = 5.3f;
    [SerializeField] private float _minWaitTime = 2.1f;
    [SerializeField] private float _maxWaitTime = 7.8f;
    [SerializeField] private float _minX = -11.5f;
    [SerializeField] private float _maxX = 11.5f;
    [SerializeField] private float _minY = -5.5f;
    [SerializeField] private float _maxY = 5.5f;

    private readonly List<GameObject> _spawnedSprites = new ();
    private GameObject _spawnedSprite;
    
    void Start() {
        StartCoroutine(LaunchSprites());
    }

    void Update() {
        // Destroy sprite if it is past the maxX
        foreach (GameObject sprite in this._spawnedSprites.ToList().Where(sprite => sprite.transform.position.x >= this._maxX)) {
            this._spawnedSprites.Remove(sprite);
            Destroy(sprite);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator LaunchSprites() {
        while (true) {
            GameObject randomSpriteRB2D = GetRandomSprite();
            this._spawnedSprite = Instantiate(randomSpriteRB2D, ResetPosition(), Quaternion.identity, this.transform);
            this._spawnedSprites.Add(this._spawnedSprite.gameObject); // Add sprite to the spawnedSprites list
            Rigidbody2D rb2d = this._spawnedSprite.GetComponent<Rigidbody2D>();
            rb2d.linearVelocity = this._spawnedSprite.transform.right * this._launchSpeed;
            rb2d.angularVelocity = this._rotationSpeed;
            yield return new WaitForSeconds(Random.Range(this._minWaitTime, this._maxWaitTime));
        }
    }
    
    private GameObject GetRandomSprite() => this._spritePrefabs[Random.Range(0, this._spritePrefabs.Length)];
    private Vector3 ResetPosition() => new(this._minX, Random.Range(this._minY, this._maxY));
}
