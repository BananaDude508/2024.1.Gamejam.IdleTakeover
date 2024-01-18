using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] shopList;
    int[] speedCosts, strengthCosts, defenceCosts, healthCosts;
    TextMeshProUGUI[] speedUpgradeTexts, strengthUpgradeTexts, defenceUpgradeTexts, healthUpgradeTexts;
    int currentIndex = 0;


    [SerializeField]
    PlayerStats stats = new PlayerStats(1, 10, 0, 100, 1000);

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI speedText, strengthText, defenceText, healthText, healthSliderText;
    public Slider healthSlider;

    void Start()
    {
        speedCosts = new int[shopList.Length];
        strengthCosts = new int[shopList.Length];
        defenceCosts = new int[shopList.Length];
        healthCosts = new int[shopList.Length];

        speedUpgradeTexts = new TextMeshProUGUI[shopList.Length];
        strengthUpgradeTexts = new TextMeshProUGUI[shopList.Length];
        defenceUpgradeTexts = new TextMeshProUGUI[shopList.Length];
        healthUpgradeTexts = new TextMeshProUGUI[shopList.Length];

        speedCosts[0] = 100;
        strengthCosts[0] = 100;
        defenceCosts[0] = 100;
        healthCosts[0] = 100;
        for (int i = 1; i < shopList.Length; i++)
        {
            speedCosts[i] = speedCosts[i-1] * 4 * i;
            strengthCosts[i] = speedCosts[i-1] * 4 * 1;
            defenceCosts[i] = speedCosts[i-1] * 4* i;
            healthCosts[i] = speedCosts[i-1] * 4 * i;
        }
        for (int i = 0; i < shopList.Length; i++)
        {
            speedUpgradeTexts[i] = shopList[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            strengthUpgradeTexts[i] = shopList[i].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            defenceUpgradeTexts[i] = shopList[i].transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            healthUpgradeTexts[i] = shopList[i].transform.GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        UpdateUIDisplay();
    }
    void UpdateUIDisplay()
    {
        // Stats
        moneyText.SetText(stats.Money().ToString());
        speedText.SetText(stats.Speed().ToString());
        strengthText.SetText(stats.Strength().ToString());
        defenceText.SetText(stats.Defence().ToString());
        healthText.SetText(stats.HealthMax().ToString());
        
        // Slider
        healthSliderText.SetText(stats.HealthCurrent()+"/"+stats.HealthMax());
        healthSlider.maxValue = stats.HealthMax();
        healthSlider.value = stats.HealthCurrent();

        // Upgrade Texts
        speedUpgradeTexts[currentIndex].SetText("Speed $"+speedCosts[currentIndex]+" for "+(int)Mathf.Round(speedCosts[currentIndex]*0.01f*(currentIndex+0.5f)));
        strengthUpgradeTexts[currentIndex].SetText("Strength $"+strengthCosts[currentIndex]+" for "+(int)Mathf.Round(strengthCosts[currentIndex]*0.05f*(currentIndex+0.5f)));
        defenceUpgradeTexts[currentIndex].SetText("Defence $"+defenceCosts[currentIndex]+" for "+(int)Mathf.Round(defenceCosts[currentIndex]*0.03f*(currentIndex+0.5f)));
        healthUpgradeTexts[currentIndex].SetText("Health $"+healthCosts[currentIndex]+" for "+(int)Mathf.Round(healthCosts[currentIndex]*0.015f*(currentIndex+0.5f)));
    }

    #region Upgrade Purchases

    public void PurchaseSpeed(int pageIndex)
    {
        int cost = speedCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f * (pageIndex + 0.5f) ); // multiplying in page index means higher
        if (stats.Money() < cost) return;                                  // tiers actually provide more value (+0.5 so index 0 has value)
        stats.ChangeMoney(-cost);                                          // Does have the issue(?) of prices and value going up heaaaps
        stats.ChangeSpeed(change);
        speedCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseStrength(int pageIndex)
    {
        int cost = strengthCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f);
        if (stats.Money() < cost) return;
        stats.ChangeMoney(-cost);
        stats.ChangeStrength(change);
        strengthCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseDefence(int pageIndex)
    {
        int cost = defenceCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f);
        if (stats.Money() < cost) return;
        stats.ChangeMoney(-cost);
        stats.ChangeDefence(change);
        defenceCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseHealth(int pageIndex)
    {
        int cost = healthCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f);
        if (stats.Money() < cost) return;
        stats.ChangeMoney(-cost);
        stats.ChangeHealth(change);
        healthCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }

    #endregion

    #region Shop Pages Handling

    #region Get Unwrapped Index Helpers

    int GetNextUnwrappedIndex()
    {
        return (int)Mathf.Min(currentIndex + 1, shopList.Length - 1);
        // if (currentIndex + 1 >= shopList.Length) return shopList.Length;
        // return currentIndex + 1;
    }

    int GetPreviousUnwrappedIndex()
    {
        return (int)Mathf.Max(currentIndex - 1, 0);
        // if (currentIndex - 1 < 0) return 0;
        // return currentIndex - 1;
    }

    #endregion

    #region Page Switchers

    public void NextShopPage()
    {
        shopList[currentIndex].SetActive(false);
        currentIndex = GetNextUnwrappedIndex();
        shopList[currentIndex].SetActive(true);
    }

    public void PreviousShopPage()
    {
        shopList[currentIndex].SetActive(false);
        currentIndex = GetPreviousUnwrappedIndex();
        shopList[currentIndex].SetActive(true);
    }

    #endregion

    #endregion

}

[System.Serializable]
public struct PlayerStats
{
    public int speed, strength, defence, healthMax, healthCurrent, money;

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