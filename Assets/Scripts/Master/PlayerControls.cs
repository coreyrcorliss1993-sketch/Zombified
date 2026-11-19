using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMasterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float minimumPitch = -85f;
    [SerializeField] private float maximumPitch = 85f;
    [SerializeField] private bool lockCursor = true;

    [Header("Animation")]
    [SerializeField] private float animationDampTime = 0.12f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float currentPlanarSpeed;
    private float pitch;
    private bool jumpRequested;
    private bool sprintHeld;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");

    public bool IsGrounded => characterController != null && characterController.isGrounded;
    public Vector3 Velocity => characterController != null ? characterController.velocity : Vector3.zero;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        UpdateLook();
        UpdateMovement();
        UpdateAnimator();
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnJump(InputValue value) { if (value.isPressed) jumpRequested = true; }
    public void OnSprint(InputValue value) => sprintHeld = value.isPressed;

    private void UpdateLook()
    {
        if (cameraPivot == null) return;

        transform.Rotate(Vector3.up, lookInput.x * mouseSensitivity, Space.Self);
        pitch = Mathf.Clamp(pitch - lookInput.y * mouseSensitivity, minimumPitch, maximumPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void UpdateMovement()
    {
        if (characterController == null) return;

        bool grounded = characterController.isGrounded;
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
        float targetSpeed = inputMagnitude == 0f ? 0f : (sprintHeld ? sprintSpeed : runSpeed);
        currentPlanarSpeed = Mathf.MoveTowards(currentPlanarSpeed, targetSpeed, acceleration * Time.deltaTime);

        Vector3 localDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        if (localDirection.sqrMagnitude > 1f) localDirection.Normalize();
        Vector3 worldDirection = transform.TransformDirection(localDirection);

        if (jumpRequested && grounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpRequested = false;

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 motion = worldDirection * currentPlanarSpeed;
        motion.y = verticalVelocity;
        characterController.Move(motion * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null || characterController == null) return;

        Vector3 planarVelocity = characterController.velocity;
        planarVelocity.y = 0f;
        Vector3 localVelocity = transform.InverseTransformDirection(planarVelocity);

        float magnitude = 0f;
        float speed = planarVelocity.magnitude;
        if (speed > 0.05f)
        {
            if (speed <= walkSpeed)
                magnitude = Mathf.InverseLerp(0f, walkSpeed, speed) * 0.5f;
            else
                magnitude = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(walkSpeed, runSpeed, speed));
        }

        Vector2 direction = new Vector2(localVelocity.x, localVelocity.z).normalized;
        animator.SetFloat(MoveXHash, direction.x * magnitude, animationDampTime, Time.deltaTime);
        animator.SetFloat(MoveYHash, direction.y * magnitude, animationDampTime, Time.deltaTime);
        if (HasParameter(GroundedHash, AnimatorControllerParameterType.Bool))
            animator.SetBool(GroundedHash, characterController.isGrounded);
        if (HasParameter(VerticalSpeedHash, AnimatorControllerParameterType.Float))
            animator.SetFloat(VerticalSpeedHash, verticalVelocity);
    }
    private bool HasParameter(int hash, AnimatorControllerParameterType type)
    {
        if (animator == null) return false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (parameter.nameHash == hash && parameter.type == type) return true;
        return false;
    }
}