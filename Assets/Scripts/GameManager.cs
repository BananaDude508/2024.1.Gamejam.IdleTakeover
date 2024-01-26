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
    int[] speedCosts, strengthCosts, defenceCosts, healthCosts, purchaseCounts;
    int speedPurchases, strengthPurchases, defencePurchases, healthPurchases = 0;
    TextMeshProUGUI[] speedUpgradeTexts, strengthUpgradeTexts, defenceUpgradeTexts, healthUpgradeTexts;
    int currentIndex = 0;

    int currentEnemy = 0;

    private float tPlayer, tEnemy;

    public Image enemySprite;
    public Sprite normalBuilding;
    public Sprite damagedBuilding;
    public Sprite veryDamagedBuilding;



    [SerializeField]
    PlayerStats stats = new PlayerStats(1, 10, 0, 100, 1000);
    [SerializeField]
    EnemyStats enemyStats = new EnemyStats(1, 10, 0, 100);

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI speedText, strengthText, defenceText, healthText, healthSliderText, enemySliderText, counterText;
    public Slider healthSlider, enemySlider;
    public Button tempSpeedButton, tempStrengthButton, tempDefenceButton, tempHealthButton;
    void OnEnable()
    {
        tPlayer = 0f;
        tEnemy = 0f;
    }

    #endregion

    void Start()
    {
        purchaseCounts = new int[]{
            1, 2, 5, 10 //, 20, 50, 100 // uncomment max purchase size
        };
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
        // for (int i = 1; i < shopList.Length; i++)
        // {
        //     speedCosts[i] = speedCosts[i-1] * 4 * i;
        //     strengthCosts[i] = speedCosts[i-1] * 4 * i;
        //     defenceCosts[i] = speedCosts[i-1] * 4 * i;
        //     healthCosts[i] = speedCosts[i-1] * 4 * i;
        // }
        // for (int i = 0; i < shopList.Length; i++)
        // {
        speedUpgradeTexts[0] = shopList[0].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        strengthUpgradeTexts[0] = shopList[0].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        defenceUpgradeTexts[0] = shopList[0].transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        healthUpgradeTexts[0] = shopList[0].transform.GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        // }
        enemyStats.NewStats(stats);
    }

    void Update()
    {
        UpdateUIDisplay();

        HurtPlayer(enemyStats.Strength(), enemyStats.Speed());
        HurtEnemy(stats.Strength(), stats.Speed());

        if (enemyStats.HealthCurrent() <= 0)
        {
            stats.ChangeMoney((int)Mathf.Round(enemyStats.HealthMax() * Random.Range(0.25f, 0.5f)));
            if (currentEnemy == 10) { currentEnemy = 0; enemyStats.NewBossStats(stats); }
            else enemyStats.NewStats(stats);
            stats.FullHeal();
            enemySprite.sprite = normalBuilding;
            currentEnemy++;
        }

        if (stats.healthCurrent <= 0)
        {
            GameOver();
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        if (enemyStats.HealthPercent() <= 0.66f) { enemySprite.sprite = damagedBuilding; }
        if (enemyStats.HealthPercent() <= 0.33f) { enemySprite.sprite = veryDamagedBuilding; }
        
    }

    float GetSummation(float startingPrice, float inflation, float firstTerm, float termsToUse)
    {
        float aN = startingPrice * Mathf.Pow(inflation, firstTerm);
        float r = inflation;
        float sN = aN * ((1 - Mathf.Pow(inflation, termsToUse)) / (1 - inflation));
        return sN;
    }

    void UpdateUIDisplay()
    {
        // Stats
        moneyText.SetText("$ "+ Mathf.Floor(stats.Money()).ToString());
        speedText.SetText(stats.Speed().ToString());
        strengthText.SetText(stats.Strength().ToString());
        defenceText.SetText(stats.Defence().ToString());
        healthText.SetText(stats.HealthMax().ToString());
        counterText.SetText("Controlling " + currentEnemy + " buildings");

        // Slider
        healthSliderText.SetText(stats.HealthCurrent() + "/" + stats.HealthMax());
        healthSlider.maxValue = stats.HealthMax();
        healthSlider.value = stats.HealthCurrent();

        // Upgrade Texts
        speedUpgradeTexts[0].SetText("Speed $" + Mathf.Ceil(GetSummation(speedCosts[0], 1.1f, speedPurchases, purchaseCounts[currentIndex])) + " for " + (int)purchaseCounts[currentIndex]);
        strengthUpgradeTexts[0].SetText("Strength $" + Mathf.Ceil(GetSummation(strengthCosts[0], 1.1f, strengthPurchases, purchaseCounts[currentIndex])) + " for " + (int)purchaseCounts[currentIndex]);
        defenceUpgradeTexts[0].SetText("Defence $" + Mathf.Ceil(GetSummation(defenceCosts[0], 1.1f, defencePurchases, purchaseCounts[currentIndex])) + " for " + (int)purchaseCounts[currentIndex]);
        healthUpgradeTexts[0].SetText("Health $" + Mathf.Ceil(GetSummation(healthCosts[0], 1.1f, healthPurchases, purchaseCounts[currentIndex])) + " for " + (int)purchaseCounts[currentIndex]);
        // Enemy
        enemySlider.maxValue = enemyStats.HealthMax();
        enemySlider.value = enemyStats.HealthMax() - enemyStats.HealthCurrent();
        enemySliderText.SetText((100f-(enemyStats.HealthPercent()*100f)).ToString("F0") + "% control");
    }

    #region Upgrade Purchases

    public void PurchaseSpeed(int pageIndex)
    {
        float cost = (GetSummation(speedCosts[0], 1.1f, speedPurchases, purchaseCounts[currentIndex]));
        int change = purchaseCounts[currentIndex];
        if (stats.Money() < cost) return;
        speedPurchases += change;
        stats.ChangeMoney(-cost);
        stats.ChangeSpeed(change);
    }
    public void PurchaseStrength(int pageIndex)
    {
        float cost = (GetSummation(strengthCosts[0], 1.1f, strengthPurchases, purchaseCounts[currentIndex]));
        int change = purchaseCounts[currentIndex];
        if (stats.Money() < cost) return;
        healthPurchases += change;
        stats.ChangeMoney(-cost);
        stats.ChangeStrength(change);
    }
    public void PurchaseDefence(int pageIndex)
    {
        float cost = (GetSummation(defenceCosts[0], 1.1f, defencePurchases, purchaseCounts[currentIndex]));
        int change = purchaseCounts[currentIndex];
        if (stats.Money() < cost) return;
        defencePurchases += change;
        stats.ChangeMoney(-cost);
        stats.ChangeDefence(change);
    }
    public void PurchaseHealth(int pageIndex)
    {
        float cost = (GetSummation(healthCosts[0], 1.1f, healthPurchases, purchaseCounts[currentIndex]));
        int change = purchaseCounts[currentIndex];
        if (stats.Money() < cost) return;
        healthPurchases += change;
        stats.ChangeMoney(-cost);
        stats.ChangeHealth(change);
    }

    #endregion

    #region Shop Pages Handling

    #region Get Unwrapped Index Helpers

    int GetNextUnwrappedIndex()
    {
        return (int)Mathf.Min(currentIndex + 1, purchaseCounts.Length - 1);
        // if (currentIndex + 1 >= purchaseCounts.Length) return purchaseCounts.Length - 1;
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
        // shopList[currentIndex].SetActive(false);
        currentIndex = GetNextUnwrappedIndex();
        // shopList[currentIndex].SetActive(true);
    }

    public void PreviousShopPage()
    {
        // shopList[currentIndex].SetActive(false);
        currentIndex = GetPreviousUnwrappedIndex();
        // shopList[currentIndex].SetActive(true);
    }

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

    #endregion
    void GameOver()
    {
        SceneManager.LoadScene("Gameover");
    }

    #region Temporary Upgrades

    public void UseSpeedItem()
    {
        StartCoroutine(SpeedItem());
    }
    IEnumerator SpeedItem()
    {
        tempSpeedButton.interactable = false;
        stats.SetSpeed((int)Mathf.Round(stats.Speed() * 1.2f));
        yield return new WaitForSeconds(15);
        stats.SetSpeed((int)Mathf.Round(stats.Speed() / 1.2f));
        yield return new WaitForSeconds(45);
        tempSpeedButton.interactable = true;
    }

    public void UseStrengthItem()
    {
        StartCoroutine(StrengthItem());
    }
    IEnumerator StrengthItem()
    {
        tempStrengthButton.interactable = false;
        stats.SetStrength((int)Mathf.Round(stats.Strength() * 1.2f));
        yield return new WaitForSeconds(15);
        stats.SetStrength((int)Mathf.Round(stats.Strength() / 1.2f));
        yield return new WaitForSeconds(45);
        tempStrengthButton.interactable = true;
    }

    public void UseDefenceItem()
    {
        StartCoroutine(DefenceItem());
    }
    IEnumerator DefenceItem()
    {
        tempDefenceButton.interactable = false;
        stats.SetDefence((int)Mathf.Round(stats.Defence() * 1.2f));
        yield return new WaitForSeconds(15);
        stats.SetDefence((int)Mathf.Round(stats.Defence() / 1.2f));
        yield return new WaitForSeconds(45);
        tempDefenceButton.interactable = true;
    }

    public void UseHealthItem()
    {
        StartCoroutine(HealthItem());
    }
    IEnumerator HealthItem()
    {
        tempHealthButton.interactable = false;
        stats.FullHeal();
        yield return new WaitForSeconds(60);
        tempHealthButton.interactable = true;
    }

    #endregion
}

[System.Serializable]
public struct PlayerStats
{
    public int speed, strength, defence, healthMax, healthCurrent;
    public float money;

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

    public void SetSpeed(int change) { speed = change; }
    public void ChangeSpeed(int change) { speed += change; }
    public int Speed() { return speed; }

    public void SetStrength(int change) { strength = change; }
    public void ChangeStrength(int change) { strength += change; }
    public int Strength() { return strength; }

    public void SetDefence(int change) { defence = change; }
    public void ChangeDefence(int change) { defence += change; }
    public int Defence() { return defence; }

    public void FullHeal() { healthCurrent = healthMax; }
    public void SetHealthMax(int change) { healthMax = change; }
    public void SetHealthCurrent(int change) { healthCurrent = change; }
    public void ChangeHealth(int change) { healthMax += change; healthCurrent += change; }
    public void ChangeHealthCurrent(int change) { healthCurrent += change; }
    public int HealthMax() { return healthMax; }
    public int HealthCurrent() { return healthCurrent; }

    public void ChangeMoney(float change) { money += change; }
    public float Money() { return money; }

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

    public void SetSpeed(int change) { speed = change; }
    public void ChangeSpeed(int change) { speed += change; }
    public int Speed() { return speed; }

    public void SetStrength(int change) { strength = change; }
    public void ChangeStrength(int change) { strength += change; }
    public int Strength() { return strength; }

    public void SetDefence(int change) { defence = change; }
    public void ChangeDefence(int change) { defence += change; }
    public int Defence() { return defence; }

    public void SetHealthMax(int change) { healthMax = change; }
    public void SetHealthCurrent(int change) { healthCurrent = change; }
    public void ChangeHealth(int change) { healthMax += change; healthCurrent += change; }
    public void ChangeHealthCurrent(int change) { healthCurrent += change; }
    public int HealthMax() { return healthMax; }
    public int HealthCurrent() { return healthCurrent; }

    public float HealthPercent() { return (float)healthCurrent / (float)healthMax; }

    #endregion

}