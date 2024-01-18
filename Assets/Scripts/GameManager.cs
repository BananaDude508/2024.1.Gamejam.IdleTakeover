using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Various Decs.

    [SerializeField]
    GameObject[] shopList;
    int[] speedCosts, strengthCosts, defenceCosts, healthCosts;
    TextMeshProUGUI[] speedUpgradeTexts, strengthUpgradeTexts, defenceUpgradeTexts, healthUpgradeTexts;
    int currentIndex = 0;

    int currentEnemy = 0;

    private float tPlayer, tEnemy;


    [SerializeField]
    PlayerStats stats = new PlayerStats(1, 10, 0, 100, 1000);
    [SerializeField]
    EnemyStats enemyStats = new EnemyStats(1, 10, 0, 100);

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI speedText, strengthText, defenceText, healthText, healthSliderText, enemySliderText;
    public Slider healthSlider, enemySlider;

    void OnEnable()
    {
        tPlayer = 0f;
        tEnemy = 0f;
    }

    #endregion

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
            strengthCosts[i] = speedCosts[i-1] * 4 * i;
            defenceCosts[i] = speedCosts[i-1] * 4 * i;
            healthCosts[i] = speedCosts[i-1] * 4 * i;
        }
        for (int i = 0; i < shopList.Length; i++)
        {
            speedUpgradeTexts[i] = shopList[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            strengthUpgradeTexts[i] = shopList[i].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            defenceUpgradeTexts[i] = shopList[i].transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            healthUpgradeTexts[i] = shopList[i].transform.GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }
        enemyStats.NewStats(stats);
        currentEnemy++;
    }

    void Update()
    {
        UpdateUIDisplay();

        HurtPlayer(enemyStats.Strength(), enemyStats.Speed());
        HurtEnemy(stats.Strength(), stats.Speed());

        if (enemyStats.HealthCurrent() <= 0)
        {
            stats.ChangeMoney((int)Mathf.Round(enemyStats.HealthMax() * Random.Range(0.25f, 0.5f)));
            if (currentEnemy == 10) {currentEnemy = 0; enemyStats.NewBossStats(stats); }
            else enemyStats.NewStats(stats);
            stats.FullHeal();
        }

        if (stats.healthCurrent <= 0)
        {
            GameOver();
        }
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

        // Enemy
        enemySlider.maxValue = enemyStats.HealthMax();
        enemySlider.value = enemyStats.HealthCurrent();
        enemySliderText.SetText(enemyStats.HealthCurrent()+"/"+enemyStats.HealthMax());
    }

    #region Upgrade Purchases

    public void PurchaseSpeed(int pageIndex)
    {
        int cost = speedCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f * (pageIndex + 0.5f)); // multiplying in page index means higher
        if (stats.Money() < cost) return;                                  // tiers actually provide more value (+0.5 so index 0 has value)
        stats.ChangeMoney(-cost);                                          // Does have the issue(?) of prices and value going up heaaaps
        stats.ChangeSpeed(change);
        speedCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseStrength(int pageIndex)
    {
        int cost = strengthCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f * (pageIndex + 0.5f));
        if (stats.Money() < cost) return;
        stats.ChangeMoney(-cost);
        stats.ChangeStrength(change);
        strengthCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseDefence(int pageIndex)
    {
        int cost = defenceCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f * (pageIndex + 0.5f));
        if (stats.Money() < cost) return;
        stats.ChangeMoney(-cost);
        stats.ChangeDefence(change);
        defenceCosts[pageIndex] = (int)Mathf.Round(cost * 1.1f);
    }
    public void PurchaseHealth(int pageIndex)
    {
        int cost = healthCosts[pageIndex];
        int change = (int)Mathf.Round(cost * 0.05f * (pageIndex + 0.5f));
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

    #region Attack Handling

    void HurtEnemy(int damage, int atkSpeed)
    {
        float dur = 1f / atkSpeed;
        tEnemy += Time.deltaTime;
        while (tEnemy >= dur)
        {
            tEnemy -= dur;
            enemyStats.ChangeHealthCurrent(-(int)Mathf.Round(damage * 0.5f));
        }
    }

    public void HurtEnemyManual()
    {
        enemyStats.ChangeHealthCurrent(-stats.Strength());
    }

    void HurtPlayer(int damage, int atkSpeed)
    {
        float dur = 1f / atkSpeed;
        tPlayer += Time.deltaTime;
        while (tPlayer >= dur)
        {
            tPlayer -= dur;
            stats.ChangeHealthCurrent((int)Mathf.Min(stats.Defence() - damage, 0));
        }
    }

    #endregion

    void GameOver()
    {
        SceneManager.LoadScene("Gameover");
    }
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

    public void FullHeal() {healthCurrent = healthMax;}
    public void SetHealthMax(int change) {healthMax = change;}
    public void SetHealthCurrent(int change) {healthCurrent = change;}
    public void ChangeHealth(int change) { healthMax += change; healthCurrent += change; }
    public void ChangeHealthCurrent(int change) { healthCurrent += change;}
    public int HealthMax () { return healthMax; }
    public int HealthCurrent() { return healthCurrent; }

    public void ChangeMoney(int change) { money += change; }
    public int Money() { return money; }

    #endregion
}

[System.Serializable]
public struct EnemyStats
{
    public int speed, strength, defence, healthMax, healthCurrent;

    public EnemyStats(int speed, int strength, int defence, int healthMax)
    {
        this.speed = speed;
        this.strength = strength;
        this.defence = defence;
        this.healthMax = healthMax;
        this.healthCurrent = healthMax;
    }

    #region New Stats

    public void NewStats(PlayerStats player)
    {
        speed = (int)Mathf.Round(player.Speed() * Random.Range(0.75f, 1.1f));
        strength = (int)Mathf.Round(player.Strength() * Random.Range(0.7f, 1f));
        defence = (int)Mathf.Round(player.Defence() * Random.Range(0.9f, 1.2f));
        healthMax = (int)Mathf.Round(player.HealthMax() * Random.Range(1f, 1.75f) + strength * Random.Range(0.25f, 0.75f) + speed * Random.Range(0.25f, 0.75f));
        healthCurrent = healthMax;
    }

    public void NewBossStats(PlayerStats player)
    {
        speed = (int)Mathf.Round(player.Speed() * Random.Range(1.15f, 2f));
        strength = (int)Mathf.Round(player.Strength() * Random.Range(1.1f, 1.9f));
        defence = (int)Mathf.Round(player.Defence() * Random.Range(1.4f, 2.4f));
        healthMax = (int)Mathf.Round(player.HealthMax() * Random.Range(1.6f, 2.75f) + strength * Random.Range(1f, 1.5f) + speed * Random.Range(1.2f, 1.6f));
        healthCurrent = healthMax;
    }

    #endregion

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

    #endregion

}