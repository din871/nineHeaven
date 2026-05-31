using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoguelikeProgressionManager : MonoBehaviour
{
    public static RoguelikeProgressionManager Instance { get; private set; }

    [Header("Timer")]
    [SerializeField] private float upgradeInterval = 60f;  
    private float currentTimer;
    private bool waitingForUpgrade = false;
    private bool gameActive = true;

    [Header("UI")]
    [SerializeField] private GameObject upgradePanel;
    private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI upgradeMessageText;
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button damageUpgradeButton;

    [Header("Player Stats")]
    [SerializeField] private int healthIncreaseAmount = 20;
    [SerializeField] private int damageIncreaseAmount = 5;
    [SerializeField]
    private int playerDamage = 20;   
    public int PlayerDamage => playerDamage;

    [Header("Enemy Scaling")]
    [SerializeField] private int maxEnemiesIncrease = 2;
    [SerializeField] private int enemyHealthIncrease = 10;
    [SerializeField] private int enemyDamageIncrease = 2;

    private int currentEnemyBaseHealth = 50;
    private int currentEnemyBaseDamage = 10;
    public int CurrentEnemyBaseHealth => currentEnemyBaseHealth;
    public int CurrentEnemyBaseDamage => currentEnemyBaseDamage;

    private PlayerHealth playerHealth;
    private EnemySpawner enemySpawner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        enemySpawner = EnemySpawner.Instance;

        if (playerHealth == null) Debug.LogError("PlayerHealth not found!");
        if (enemySpawner == null) Debug.LogError("EnemySpawner not found!");

        upgradePanel.SetActive(false);
        healthUpgradeButton.onClick.AddListener(() => ChooseUpgrade(true));
        damageUpgradeButton.onClick.AddListener(() => ChooseUpgrade(false));

        currentTimer = upgradeInterval;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!gameActive || waitingForUpgrade) return;

        currentTimer -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTimer <= 0f)
        {
            TriggerUpgradePhase();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"Next upgrade: {Mathf.CeilToInt(currentTimer)}s";
    }

    private void TriggerUpgradePhase()
    {
        waitingForUpgrade = true;
        gameActive = false;

        Time.timeScale = 0f;

        upgradePanel.SetActive(true);
        upgradeMessageText.text = "Выбери улучшение";

        ScaleEnemiesAndMaxCount();
    }

    private void ScaleEnemiesAndMaxCount()
    {
        if (enemySpawner != null)
            enemySpawner.IncreaseMaxEnemies(maxEnemiesIncrease);

        currentEnemyBaseHealth += enemyHealthIncrease;
        currentEnemyBaseDamage += enemyDamageIncrease;

        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            enemy.ApplyScaling(enemyHealthIncrease, enemyDamageIncrease);
        }
    }

    private void ChooseUpgrade(bool isHealthUpgrade)
    {
        if (isHealthUpgrade)
        {
            if (playerHealth != null)
                playerHealth.IncreaseMaxHealth(healthIncreaseAmount);
        }
        else
        {
            playerDamage += damageIncreaseAmount;
        }

        Time.timeScale = 1f;
        upgradePanel.SetActive(false);
        waitingForUpgrade = false;
        gameActive = true;

        currentTimer = upgradeInterval;
        UpdateTimerUI();
    }

    public void OnPlayerDeath()
    {
        gameActive = false;
        waitingForUpgrade = false;
        if (upgradePanel.activeSelf)
            upgradePanel.SetActive(false);
        Time.timeScale = 0f; 
    }
}