using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private GameOverManager gameOverManager;

    private void Start()
    {
        currentHealth = maxHealth;
        if (gameOverManager == null)
        {
            gameOverManager = FindObjectOfType<GameOverManager>();
            if (gameOverManager == null)
            {
                Debug.LogError("GameOverManager not found!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GetComponent<PlayerController>().enabled = false;
        gameOverManager.ShowGameOverMenu();

        if (RoguelikeProgressionManager.Instance != null)
            RoguelikeProgressionManager.Instance.OnPlayerDeath();

        Debug.Log("Player Died!");
        Destroy(gameObject);
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}