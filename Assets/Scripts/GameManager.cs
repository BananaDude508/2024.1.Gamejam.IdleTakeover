using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    int shopPages = 1;
    int[] speedCosts, strengthCosts, defenceCosts, healthCosts;


    PlayerStats stats = new PlayerStats(1, 10, 0, 100, 1000);

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI speedText, strengthText, defenceText, healthText;
    public Slider healthSlider;

    void Start()
    {
        speedCosts = new int[shopPages];
        strengthCosts = new int[shopPages];
        defenceCosts = new int[shopPages];
        healthCosts = new int[shopPages];

        speedCosts[0] = 100;
        strengthCosts[0] = 100;
        defenceCosts[0] = 100;
        healthCosts[0] = 100;
        for (int i = 1; i < shopPages; i++)
        {
            speedCosts[i] = speedCosts[i-1] * 9 * i;
            strengthCosts[i] = speedCosts[i-1] * 9 * 1;
            defenceCosts[i] = speedCosts[i-1] * 9 * i;
            healthCosts[i] = speedCosts[i-1] * 9 * i;
        }


    }

    void Update()
    {
        UpdateUIDisplay();
    }

    #region Upgrade Purchases

    public void PurchaseSpeed(int pageIndex)
    {
        int cost = speedCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f * (pageIndex + 0.5f) ); // multiplying in page index means higher
        if (stats.Money() < cost) return;                                 // tiers actually provide more value (+0.5 so index 0 has value)
        stats.ChangeSpeed(change);
        speedCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }

    public void PurchaseStrength(int pageIndex)
    {
        int cost = strengthCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f);
        if (stats.Money() < cost) return;
        stats.ChangeStrength(change);
        strengthCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseDefence(int pageIndex)
    {
        int cost = defenceCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f);
        if (stats.Money() < cost) return;
        stats.ChangeDefence(change);
        defenceCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseHealth(int pageIndex)
    {
        int cost = healthCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f);
        if (stats.Money() < cost) return;
        stats.ChangeHealth(change);
        healthCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }

    void UpdateUIDisplay()
    {
        moneyText.SetText(stats.Money().ToString());
        speedText.SetText(stats.Speed().ToString());
        strengthText.SetText(stats.Strength().ToString());
        defenceText.SetText(stats.Defence().ToString());
        healthText.SetText(stats.HealthCurrent().ToString());
        healthSlider.value = stats.HealthMax() / stats.HealthCurrent() * 0.01f;
    }

    #endregion

    #region Get Unwrapped Index Helpers

    int GetNextUnwrappedIndex(int currentIndex)
    {
        return Mathf.Min(currentIndex + 1, shopPages);
        // if (currentIndex + 1 >= shopPages) return shopPahes;
        // return currentIndex + 1;
    }

    int GetPreviousUnwrappedIndex(int currentIndex)
    {
        return Mathf.Max(currentIndex - 1, 0);
        // if (currentIndex - 1 < 0) return 0;
        // return currentIndex - 1;
    }

    #endregion

}

public struct PlayerStats {
    int speed, strength, defence, healthMax, healthCurrent, money;

    public PlayerStats(int speed, int strength, int defence, int healthMax, int money)
    {
        this.speed = speed;
        this.strength = strength;
        this.defence = defence;
        this.healthMax = healthMax;
        this.healthCurrent = healthMax;
        this.money = money;
    }

    #region Get/Set Stats

    public void SetSpeed(int change) {speed = change;}
    public void ChangeSpeed(int change) { speed += change; }
    public int Speed() { return speed; }

    public void SetStrength(int change) {strength = change;}
    public void ChangeStrength(int change) { strength += change; }
    public int Strength() { return strength; }

    public void SetDefence(int change) {defence = change;}
    public void ChangeDefence(int change) { defence += change; }
    public int Defence() { return defence; }

    public void SetHealthMax(int change) {healthMax = change;}
    public void SetHealthCurrent(int change) {healthCurrent = change;}
    public void ChangeHealth(int change) { healthMax += change; healthCurrent += change; }
    public void ChangeHealthCurrent(int change) { healthCurrent += change; }
    public int HealthMax () { return healthMax; }
    public int HealthCurrent() { return healthCurrent; }

    public void ChangeMoney(int change) { money += change; }
    public int Money() { return money; }

    #endregion
}