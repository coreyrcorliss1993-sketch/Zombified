using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 20f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.75f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;

    private float nextAttackTime;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    private void Start()
    {
        if (target != null)
        {
            playerHealth = target.GetComponent<PlayerHealth>();
        }
    }


    private void Update()
    {
        if (target == null || playerHealth == null)
        {
            return;
        }

        if (playerHealth.IsDead)
        {
            StopMoving();
            return;
        }

        float distanceToTarget = Vector3.Distance(
            transform.position,
            target.position
        );

        if (distanceToTarget <= attackRange)
        {
            Attack();
        }
        else if (distanceToTarget <= detectionRange)
        {
            ChaseTarget();
        }
        else
        {
            StopMoving();
        }
    }


    private void ChaseTarget()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }


    private void Attack()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;

        FaceTarget();

        if (Time.time >= nextAttackTime)
        {
            playerHealth.TakeDamage(attackDamage);

            nextAttackTime =
                Time.time + attackCooldown;
        }
    }


    private void FaceTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            8f * Time.deltaTime
        );
    }


    private void StopMoving()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;
    }
}