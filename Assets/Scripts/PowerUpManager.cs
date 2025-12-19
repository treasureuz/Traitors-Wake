using UnityEngine;

public class PowerUpManager : MonoBehaviour {
    public static PowerUpManager instance;
    
    void Awake() {
        instance = this;
    }
    
    public void ActivatePowerUp() {
        Debug.Log("Activating PowerUp");
    }
}
