using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Player _aiPlayerPrefab;
    public static GameManager instance;
    
    void Start() {
        instance = this;
        Instantiate(this._aiPlayerPrefab, this._aiPlayerPrefab.transform.position, Quaternion.identity);
    }
}
