using UnityEngine;

public class SurvivalistAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;

    [Header("Animation Smoothing")]
    [SerializeField] private float dampTime = 0.12f;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (animator == null || characterController == null)
            return;

        // Get the player's actual world movement.
        Vector3 worldVelocity = characterController.velocity;

        // Ignore jumping/falling for ground locomotion.
        worldVelocity.y = 0f;

        // Convert world movement into movement relative to the Player.
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

        float speed = localVelocity.magnitude;

        float moveX = 0f;
        float moveY = 0f;

        if (speed > 0.05f)
        {
            // Determine whether we should be in the walk or run area
            // of the 2D Blend Tree.
            float animationMagnitude;

            if (speed <= walkSpeed)
            {
                animationMagnitude = Mathf.Lerp(
                    0f,
                    0.5f,
                    speed / walkSpeed
                );
            }
            else
            {
                animationMagnitude = Mathf.Lerp(
                    0.5f,
                    1f,
                    Mathf.InverseLerp(walkSpeed, runSpeed, speed)
                );
            }

            // Normalize direction so X/Y describe direction only.
            Vector2 direction = new Vector2(
                localVelocity.x,
                localVelocity.z
            ).normalized;

            moveX = direction.x * animationMagnitude;
            moveY = direction.y * animationMagnitude;
        }

        // Send movement to the Animator.
        animator.SetFloat(
            MoveXHash,
            moveX,
            dampTime,
            Time.deltaTime
        );

        animator.SetFloat(
            MoveYHash,
            moveY,
            dampTime,
            Time.deltaTime
        );
    }
}