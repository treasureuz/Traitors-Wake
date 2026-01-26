using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : MonoBehaviour {
    [SerializeField] private float _minCompleteTime = 3f;
    [SerializeField] private float _maxCompleteTime = 8f;
    [SerializeField] private int _minHealPoints = 5;
    [SerializeField] private int _maxHealPoints = 15;
    
    private readonly List<PowerUp> _activatedPowerUps = new();
    private int _currentCollectedChests;
    
    public int healPoints { get; private set; }
    public int ammo { get; private set; }
    public float addedTime { get; private set; }
    
    public int totalHealPoints { get; private set; }
    private readonly List<int> _totalHealPointsList = new();
    public int totalAmmo { get; private set; }
    private readonly List<int> _totalAmmoList = new();
    public float totalAddedTime { get; private set; }
    private readonly List<float> _totalAddedTimeList = new();

    public bool hasClearedObstacles { get; private set; }
    public bool hasTraitorsWake { get; private set; }
    
    public enum PowerUp {
        AmmoSurplus = 0, 
        TraitorsWake = 1, 
        BonusTime = 2, 
        ClearObstacles = 3,
        HealthBoost = 4,
        None
    }
    public PowerUp powerUp { get; private set; } = PowerUp.None;
    
    public void ActivatePowerUp() {
        // Get random number between 0 - PowerUp.length - 2 (4)
        var randomPowerUpNum = Random.Range(0, Enum.GetValues(typeof(PowerUp)).Length - 1); 
        this.powerUp = (PowerUp) randomPowerUpNum;
        HandlePowerUps();
        UIManager.instance.UpdateHotBarFeedText(); // Updates feed text based on given power up
    }

    private void HandlePowerUps() {
        this._currentCollectedChests++;
        this._activatedPowerUps.Add(this.powerUp);
        switch (this.powerUp) {
            case PowerUp.AmmoSurplus: {
                // Amount of ammo to give the player (between 1 - maxMagCount;) 
                this.ammo = Random.Range(1, GameManager.instance.pWeaponManager.GetMaxMagazineCount() + 1); 
                this.totalAmmo += this.ammo;
                this._totalAmmoList.Add(this.totalAmmo);
                GameManager.instance.pWeaponManager.SetCurrentMagazineCount
                    (GameManager.instance.pWeaponManager.GetCurrentMagazineCount() + this.ammo);
                UIManager.instance.UpdateGiveAmmoText();
                UIManager.instance.UpdateBulletBar(); // Enables all bullet bars up until currentMagCount
                break;
            }
            case PowerUp.TraitorsWake: {
                // Enable the AIManager.LineRenderer after timeToMemorize is done
                this.hasTraitorsWake = true;
                GameManager.instance.traitor.SetLineRendererStatus(true);
                UIManager.instance.EnableTraitorsLineSprite();
                break;
            }
            case PowerUp.BonusTime: {
                // Amount to increase the timeToComplete by (between minCompleteTime(3) - maxCompleteTime(8))
                this.addedTime = Random.Range(this._minCompleteTime, this._maxCompleteTime); 
                this.totalAddedTime += this.addedTime;
                this._totalAddedTimeList.Add(this.totalAddedTime);
                GameManager.instance.SetTimeToComplete(GameManager.instance.timeToComplete + this.addedTime);
                UIManager.instance.UpdateTimeIncreaseText();
                break;
            }
            case PowerUp.ClearObstacles: {
                this.hasClearedObstacles = true;
                GridManager.instance.ClearObstacleTiles();
                UIManager.instance.EnableRockSprites();
                break;
            }
            case PowerUp.HealthBoost: {
                // Amount of health to grant the player (between minHeal(5) - maxHeal(15))
                this.healPoints = Random.Range(this._minHealPoints, this._maxHealPoints);
                this.totalHealPoints += this.healPoints;
                this._totalHealPointsList.Add(this.totalHealPoints);
                GameManager.instance.player.HealPlayer(this.healPoints);
                UIManager.instance.UpdateHealthBoostText();
                break;
            }
            case PowerUp.None: break;
        }
    }

    public void UndoStolenPowerUps() {
        if (this._activatedPowerUps.Count == 0) return;
        var endIndex = this._activatedPowerUps.Count - this._currentCollectedChests;
        for (var i = this._activatedPowerUps.Count - 1; i >= endIndex; --i) {
            PowerUp power = this._activatedPowerUps[i];
            this._activatedPowerUps.RemoveAt(i);
            switch (power) {
                case PowerUp.AmmoSurplus: UndoTotalAmmo(); break;
                case PowerUp.BonusTime: UndoTotalAddedTime(); break;
                case PowerUp.HealthBoost: UndoTotalHealPoints(); break;
            }
            if (this._activatedPowerUps.Contains(power)) continue;
            switch (power) {
                case PowerUp.ClearObstacles: hasClearedObstacles = false; break;
                case PowerUp.TraitorsWake: hasTraitorsWake = false; break;
            }
        }
        this.powerUp = this._activatedPowerUps.Count == 0 ? PowerUp.None : this._activatedPowerUps[^1];
        ResetCurrentCollectedChests();
        UIManager.instance.UpdatePowerUpsUI();
    }

    private void UndoTotalAmmo() {
        var itemToRemove = this._totalAmmoList[^1];
        this._totalAmmoList.Remove(itemToRemove);
        if (this._totalAmmoList.Count == 0) return;
        var itemToAssign = this._totalAmmoList[^1];
        this.ammo = itemToRemove - itemToAssign;
        this.totalAmmo = itemToAssign;
    }
    
    private void UndoTotalAddedTime() {
        var itemToRemove = this._totalAddedTimeList[^1];
        this._totalAddedTimeList.Remove(itemToRemove);
        if (this._totalAddedTimeList.Count == 0) return;
        var itemToAssign = this._totalAddedTimeList[^1];
        this.addedTime = itemToRemove - itemToAssign;
        this.totalAddedTime = itemToAssign;
    }
    
    private void UndoTotalHealPoints() {
        var itemToRemove = this._totalHealPointsList[^1];
        this._totalHealPointsList.Remove(itemToRemove);
        if (this._totalHealPointsList.Count == 0) return;
        var itemToAssign = this._totalHealPointsList[^1];
        this.healPoints = itemToRemove - itemToAssign;
        this.totalHealPoints = itemToAssign;
    }
    
    public void ResetPowerUpsSettings() {
        this._activatedPowerUps.Clear();
        this._totalAddedTimeList.Clear();
        this._totalAmmoList.Clear();
        this._totalHealPointsList.Clear();
        this.powerUp = PowerUp.None;
        this.addedTime = 0; this.totalAddedTime = 0;
        this.ammo = 0; this.totalAmmo = 0;
        this.healPoints = 0; this.totalHealPoints = 0;
        this.hasClearedObstacles = false;
        this.hasTraitorsWake = false;
        UIManager.instance.DisableAllPowerUpSprites(); // Disable hotbar sprites
    }
    public List<PowerUp> GetActivatedPowerUps() => new (this._activatedPowerUps);
    public void ResetCurrentCollectedChests() => this._currentCollectedChests = 0;
}
