using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour {
    [SerializeField] private float _minCompleteTime = 3f;
    [SerializeField] private float _maxCompleteTime = 8f;
    [SerializeField] private int _minHealPoints = 10;
    [SerializeField] private int _maxHealPoints = 50;
    
    private enum PowerUp {
        GiveAmmo = 0, 
        LineTrace = 1, 
        IncreaseCompleteTime = 2, 
        ClearObstacles = 3,
        HealthBoost = 4
    }
    private PowerUp _powerUp;
    
    public void ActivatePowerUp() {
        Debug.Log("Activating PowerUp");
        // Get random number between 0 - PowerUp.length - 1 (4)
        var randomPowerUpNum = Random.Range(0, Enum.GetValues(typeof(PowerUp)).Length); 
        this._powerUp = (PowerUp) randomPowerUpNum;
        Debug.Log(this._powerUp);
        HandlePowerUps();
    }

    private void HandlePowerUps() {
        switch (this._powerUp) {
            case PowerUp.GiveAmmo: {
                var maxNum = GameManager.instance.pWeaponManager.GetMaxMagazineCount() -
                             GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
                var randomNum = Random.Range(1, maxNum + 1); // Amount of ammo to give the player (between 1 - maxNum) 
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount
                    (GameManager.instance.pWeaponManager.GetCurrentMagazineCount() + randomNum);
                UIManager.instance.UpdateBulletBar(true); // Enables all bullet bars until currentMagCount
                Debug.Log($"Gave {randomNum} ammo");
                break;
            }
            case PowerUp.LineTrace: {
                // Enable the AIManager.LineRenderer after timeToMemorize is done
                GameManager.instance.traitor.SetLineRendererStatus(true);
                break;
            }
            case PowerUp.IncreaseCompleteTime: {
                // Amount to increase the timeToComplete by (between minCompleteTime(3) - maxCompleteTime(8))
                var randomNum = Random.Range(this._minCompleteTime, this._maxCompleteTime); 
                GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete + randomNum);
                break;
            }
            case PowerUp.ClearObstacles: {
                Debug.Log("Clearing obstacles");
                GridManager.instance.ClearObstacleTiles();
                break;
            }
            case PowerUp.HealthBoost: {
                // Amount of health to grant the player (between minHeal(10) - maxHeal(50))
                var randomNum = Random.Range(this._minHealPoints, this._maxHealPoints);
                GameManager.instance.player.HealPlayer(randomNum);
                Debug.Log($"Gave {randomNum} health");
                break;
            }
        }
    }
}
