using UnityEngine;

public class PowerUpManager : MonoBehaviour {

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
        var randomPowerUpNum = Random.Range(0, 5); // Get random number between 0 - PowerUp.length (4)
        this._powerUp = (PowerUp) randomPowerUpNum;
        Debug.Log(this._powerUp);
        HandlePowerUps();
    }

    private void HandlePowerUps() {
        switch (this._powerUp) {
            case PowerUp.GiveAmmo: {
                var maxNum = GameManager.instance.pWeaponManager.GetMaxMagazineCount() -
                             GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
                var randomNum = Random.Range(1, maxNum + 1); // Amount of ammo to give the player (between 0 - maxNum) 
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount(randomNum);
                UIManager.instance.IncreaseBulletBar(GameManager.instance.pWeaponManager.GetCurrentMagazineCount() - randomNum);
                Debug.Log($"Gave {randomNum} ammo");
                break;
            }
            case PowerUp.LineTrace: {
                // Enable the AIManager.LineRenderer after timeToMemorize is done
                GameManager.instance.traitor.SetLineRendererStatus(true);
                break;
            }
            case PowerUp.IncreaseCompleteTime: {
                var randomNum = Random.Range(3, 9); // Amount to increase the timeToComplete by (between 3 - 8)
                GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete + randomNum);
                break;
            }
            case PowerUp.ClearObstacles: {
                Debug.Log("Clearing obstacles");
                GridManager.instance.ClearObstacleTiles();
                break;
            }
            case PowerUp.HealthBoost: {
                var randomNum = Random.Range(0, 2) * 40 + 10;
                GameManager.instance.player.HealPlayer(randomNum);
                Debug.Log($"Gave {randomNum} health");
                break;
            }
        }
    }
}
