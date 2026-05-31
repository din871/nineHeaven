using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Ranged Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool autoAim = true;

    private Rigidbody2D rb;
    private float horizontalInput;
    private float verticalInput;
    private float nextAttackTime;
    private bool isFacingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        if (attackPoint == null)
        {
            attackPoint = transform.Find("AttackPoint")?.transform;
            if (attackPoint == null)
                Debug.LogError("AttackPoint not found!");
        }
    }

    private void Update()
    {
        HandleMovementInput();
        HandleAutoAttack();
    }

    private void HandleMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            Flip();
        }
    }

    private void HandleAutoAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackDelay;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            verticalInput * moveSpeed
        );
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (attackPoint != null)
        {
            attackPoint.localPosition = new Vector3(
                -attackPoint.localPosition.x,
                attackPoint.localPosition.y,
                attackPoint.localPosition.z
            );
        }
    }

    private void Attack()
    {
        if (projectilePrefab == null || attackPoint == null) return;

        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        List<Collider2D> enemies = new List<Collider2D>();

        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                enemies.Add(col);
            }
        }

        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;

        if (autoAim && enemies.Count > 0)
        {
            Transform target = FindNearestEnemy(enemies.ToArray());
            if (target != null)
            {
                shootDirection = (target.position - attackPoint.position).normalized;
            }
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            attackPoint.position,
            Quaternion.identity
        );

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            int playerDamage = RoguelikeProgressionManager.Instance != null
                ? RoguelikeProgressionManager.Instance.PlayerDamage
                : 20;

            projectileScript.Initialize(shootDirection, projectileSpeed, enemyTag, playerDamage);
        }
        else
        {
            Debug.LogError("Projectile prefab missing Projectile component!");
        }
    }

    private Transform FindNearestEnemy(Collider2D[] enemies)
    {
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        return nearestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 0.1f);
        }
    }
}