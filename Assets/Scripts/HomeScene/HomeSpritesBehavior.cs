using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public partial class HomeSpritesBehavior : MonoBehaviour {
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
        foreach (GameObject sprite in this._spawnedSprites.ToList().Where(sprite => sprite.transform.position.x >= this._maxX)) {
            this._spawnedSprites.Remove(sprite);
            Destroy(sprite);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [SuppressMessage("ReSharper", "FunctionRecursiveOnAllPaths")]
    private IEnumerator LaunchSprites() {
        GameObject randomSprite = GetRandomSprite();
        this._spawnedSprite = Instantiate(randomSprite, ResetPosition(), Quaternion.identity);
        this._spawnedSprite.transform.SetParent(this.transform);
        this._spawnedSprites.Add(this._spawnedSprite);
        Rigidbody2D spriteRB2D = this._spawnedSprite.GetComponent<Rigidbody2D>();
        spriteRB2D.linearVelocity = this._spawnedSprite.transform.right * this._launchSpeed;
        spriteRB2D.angularVelocity = this._rotationSpeed;
        yield return new WaitForSeconds(Random.Range(this._minWaitTime, this._maxWaitTime));
        StartCoroutine(LaunchSprites());
    }
    
    private GameObject GetRandomSprite() => this._spritePrefabs[Random.Range(0, this._spritePrefabs.Length)].gameObject;
    private Vector3 ResetPosition() => new(this._minX, Random.Range(this._minY, this._maxY));
}
