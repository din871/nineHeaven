using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float rotateSpeed = 800f;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float lifetime = 4f;

    private int damage;
    private Transform target;
    private Rigidbody2D rb;
    private string enemyTag;

    public void Initialize(Vector2 direction, float speed, string targetTag, int damageAmount)
    {
        rb = GetComponent<Rigidbody2D>();
        this.speed = speed;
        enemyTag = targetTag;
        damage = damageAmount;
        transform.up = direction;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        UpdateTarget();
        MoveTowardsTarget();
    }

    private void UpdateTarget()
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        List<Collider2D> enemies = new List<Collider2D>();

        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag(enemyTag))
            {
                enemies.Add(col);
            }
        }

        target = GetPriorityTarget(enemies.ToArray());
    }

    private Transform GetPriorityTarget(Collider2D[] enemies)
    {
        Transform priorityTarget = null;
        float highestPriority = float.MinValue;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy == null) continue;

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            float priority = 1 / distance;

            if (priority > highestPriority)
            {
                highestPriority = priority;
                priorityTarget = enemy.transform;
            }
        }

        return priorityTarget;
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
        {
            rb.linearVelocity = transform.up * speed;
            return;
        }

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}