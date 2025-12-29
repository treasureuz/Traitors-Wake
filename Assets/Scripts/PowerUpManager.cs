using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour {
    [SerializeField] private float _minCompleteTime = 3f;
    [SerializeField] private float _maxCompleteTime = 8f;
    [SerializeField] private int _minHealPoints = 10;
    [SerializeField] private int _maxHealPoints = 50;

    public static PowerUpManager instance;
    
    public int healPoints { get; private set; }
    public int ammo { get; private set; }
    public float addedTime { get; private set; }
    
    public int totalHealPoints { get; private set; }
    public int totalAmmo { get; private set; }
    public float totalAddedTime { get; private set; }

    public bool hasClearedObstacles { get; private set; }
    public bool isLineTrace { get; private set; }
    
    public enum PowerUp {
        GiveAmmo = 0, 
        LineTrace = 1, 
        IncreaseCompleteTime = 2, 
        ClearObstacles = 3,
        HealthBoost = 4
    }
    public PowerUp powerUp { get; private set; }
    
    void Awake() {
        instance = this;
    }
    
    public void ActivatePowerUp() {
        // Get random number between 0 - PowerUp.length - 1 (4)
        var randomPowerUpNum = Random.Range(0, Enum.GetValues(typeof(PowerUp)).Length); 
        this.powerUp = (PowerUp) randomPowerUpNum;
        HandlePowerUps();
        UIManager.instance.UpdateHotBarFeedText(); // Updates feed text based on given power up
    }

    private void HandlePowerUps() {
        switch (this.powerUp) {
            case PowerUp.GiveAmmo: {
                var maxNum = GameManager.instance.pWeaponManager.GetMaxMagazineCount() -
                             GameManager.instance.pWeaponManager.GetCurrentMagazineCount();
                this.ammo = Random.Range(1, maxNum + 1); // Amount of ammo to give the player (between 1 - maxNum) 
                this.totalAmmo += this.ammo;
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount
                    (GameManager.instance.pWeaponManager.GetCurrentMagazineCount() + this.ammo);
                UIManager.instance.UpdateGiveAmmoText();
                UIManager.instance.UpdateBulletBar(true); // Enables all bullet bars up until currentMagCount
                break;
            }
            case PowerUp.LineTrace: {
                // Enable the AIManager.LineRenderer after timeToMemorize is done
                this.isLineTrace = true;
                GameManager.instance.traitor.SetLineRendererStatus(true);
                UIManager.instance.EnableTraitorsLineSprite();
                break;
            }
            case PowerUp.IncreaseCompleteTime: {
                // Amount to increase the timeToComplete by (between minCompleteTime(3) - maxCompleteTime(8))
                this.addedTime = Random.Range(this._minCompleteTime, this._maxCompleteTime); 
                this.totalAddedTime += this.addedTime;
                GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete + this.addedTime);
                UIManager.instance.UpdateTimeIncreaseText();
                break;
            }
            case PowerUp.ClearObstacles: {
                this.hasClearedObstacles = true;
                GridManager.instance.ClearObstacleTiles();
                UIManager.instance.EnableRockSprite();
                break;
            }
            case PowerUp.HealthBoost: {
                // Amount of health to grant the player (between minHeal(10) - maxHeal(50))
                this.healPoints = Random.Range(this._minHealPoints, this._maxHealPoints);
                this.totalHealPoints += this.healPoints;
                GameManager.instance.player.HealPlayer(this.healPoints);
                UIManager.instance.UpdateHealthBoostText();
                break;
            }
        }
    }

    public void ResetPowerUps() {
        UIManager.instance.DisableAllPowerUpSprites(); // Disable hotbar sprites
        this.totalAddedTime = 0;
        this.totalAmmo = 0;
        this.totalHealPoints = 0;
        this.hasClearedObstacles = false;
        this.isLineTrace = false;
    }
}
