using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Animation")]
    [SerializeField] private PlayerCombatAnimator combatAnimator;

    [Header("Melee")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [Header("Knockback")]
    [SerializeField] private float knockbackDistance = 1f;

    private PlayerInput playerInput;
    private InputAction fireAction;

    private float nextAttackTime;
    private bool attackInProgress;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        fireAction =
            playerInput.actions.FindAction("Fire");

        if (fireAction == null)
        {
            Debug.LogError(
                "PlayerMelee could not find the Fire action.",
                gameObject
            );
        }
    }


    private void Update()
    {
        if (fireAction == null)
        {
            return;
        }

        if (fireAction.WasPressedThisFrame() &&
            Time.time >= nextAttackTime &&
            !attackInProgress)
        {
            StartAttack();
        }
    }


    private void StartAttack()
    {
        attackInProgress = true;

        nextAttackTime =
            Time.time + attackCooldown;

        if (combatAnimator != null)
        {
            combatAnimator.PlayAttack();
        }
        else
        {
            Debug.LogWarning(
                "PlayerMelee has no Combat Animator assigned.",
                gameObject
            );

            attackInProgress = false;
        }
    }


    // Called by an Animation Event
    // at the exact impact frame.
    public void MeleeImpact()
    {
        if (!attackInProgress)
        {
            return;
        }

        CheckForMeleeHit();
    }


    // Called by an Animation Event
    // near the end of the attack.
    public void MeleeAttackFinished()
    {
        attackInProgress = false;
    }


    private void CheckForMeleeHit()
    {
        if (playerCamera == null)
        {
            Debug.LogError(
                "PlayerMelee does not have Main Camera assigned.",
                gameObject
            );

            return;
        }

        Vector3 origin =
            playerCamera.transform.position;

        Vector3 direction =
            playerCamera.transform.forward;

        bool hitSomething =
            Physics.SphereCast(
                origin,
                hitRadius,
                direction,
                out RaycastHit hit,
                attackRange,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            );

        Debug.DrawRay(
            origin,
            direction * attackRange,
            Color.red,
            1f
        );

        if (!hitSomething)
        {
            Debug.Log("Melee missed on impact.");
            return;
        }

        Debug.Log(
            $"Melee impact struck: {hit.collider.name}"
        );

        ZombieHealth zombieHealth =
            hit.collider
                .GetComponentInParent<ZombieHealth>();

        if (zombieHealth == null)
        {
            Debug.Log(
                $"Impact hit {hit.collider.name}, " +
                "but no ZombieHealth was found."
            );

            return;
        }

        zombieHealth.TakeDamage(damage);

        Vector3 knockbackDirection =
            zombieHealth.transform.position -
            transform.position;

        zombieHealth.ApplyKnockback(
            knockbackDirection,
            knockbackDistance
        );

        Debug.Log(
            $"MELEE IMPACT: " +
            $"{zombieHealth.gameObject.name} " +
            $"took {damage} damage."
        );
    }


    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Vector3 endPoint =
            playerCamera.transform.position +
            playerCamera.transform.forward *
            attackRange;

        Gizmos.DrawWireSphere(
            endPoint,
            hitRadius
        );
    }
}