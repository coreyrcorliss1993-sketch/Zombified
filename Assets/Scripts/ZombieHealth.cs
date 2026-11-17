using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;

    [Header("Death")]
    [SerializeField] private float destroyDelay = 1.5f;

    private float currentHealth;
    private bool isDead;

    private NavMeshAgent agent;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;


    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        agent = GetComponent<NavMeshAgent>();
    }


    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0f)
        {
            return;
        }

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"Zombie took {damage} damage. " +
            $"Health: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        currentHealth = 0f;

        Debug.Log("ZOMBIE DIED");

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        ZombieController controller =
            GetComponent<ZombieController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        Destroy(gameObject, destroyDelay);
    }

    public void ApplyKnockback(Vector3 direction, float distance)
    {
        NavMeshAgent agent =
            GetComponent<NavMeshAgent>();

        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        direction.Normalize();

        agent.Move(direction * distance);
    }
}