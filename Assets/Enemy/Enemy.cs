using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int maxHealth = 50;

    [Header("Идти к ")]
    [SerializeField] private Transform playerTransform;

    private float lastAttackTime;
    private bool isActive = true;
    private int currentHealth;

    public void Initialize(int health, int attackDamage)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        damage = attackDamage;
    }

    public void ApplyScaling(int extraHealth, int extraDamage)
    {
        maxHealth += extraHealth;
        currentHealth += extraHealth;
        damage += extraDamage;
    }

    private void Start()
    {
        if (currentHealth == 0)
            currentHealth = maxHealth;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (!isActive || playerTransform == null) return;

        HandleMovement();
        HandleAttack();
    }

    private void HandleMovement()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > attackRange) return;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            TryDamagePlayer();
            lastAttackTime = Time.time;
        }
    }

    private void TryDamagePlayer()
    {
        if (playerTransform.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Нанесено {damage} урона игроку");
        }
    }

    public void TakeDamage(int incomingDamage)
    {
        if (!isActive) return;

        currentHealth -= incomingDamage;
        Debug.Log($"Враг получил {incomingDamage} урона. Осталось здоровья: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isActive = false;

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.EnemyDied();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}