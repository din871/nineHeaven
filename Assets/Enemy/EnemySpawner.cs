using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Настройки")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 20;
    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private float spawnDistance = 12f;

    private Camera mainCamera;
    private int currentEnemies;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemies), 0f, spawnDelay);
    }

    public void IncreaseMaxEnemies(int increment)
    {
        maxEnemies += increment;
    }

    private void SpawnEnemies()
    {
        if (currentEnemies >= maxEnemies) return;

        Vector2 spawnPos = GetSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

        if (enemyScript != null && RoguelikeProgressionManager.Instance != null)
        {
            int baseHealth = RoguelikeProgressionManager.Instance.CurrentEnemyBaseHealth;
            int baseDamage = RoguelikeProgressionManager.Instance.CurrentEnemyBaseDamage;
            enemyScript.Initialize(baseHealth, baseDamage);
        }

        currentEnemies++;
    }

    private Vector2 GetSpawnPosition()
    {
        int side = Random.Range(0, 4);
        Vector2 viewportPoint = Vector2.zero;

        switch (side)
        {
            case 0:
                viewportPoint = new Vector2(0, Random.Range(0.1f, 0.9f));
                break;
            case 1:
                viewportPoint = new Vector2(1, Random.Range(0.1f, 0.9f));
                break;
            case 2:
                viewportPoint = new Vector2(Random.Range(0.1f, 0.9f), 1);
                break;
            case 3:
                viewportPoint = new Vector2(Random.Range(0.1f, 0.9f), 0);
                break;
        }

        Vector2 worldPos = mainCamera.ViewportToWorldPoint(viewportPoint);
        Vector2 direction = (Vector2.zero - worldPos).normalized;
        return worldPos + direction * spawnDistance;
    }

    public void EnemyDied()
    {
        currentEnemies--;
    }
}