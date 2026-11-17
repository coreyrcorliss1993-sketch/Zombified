using UnityEngine;
using UnityEngine.InputSystem;

public class CombatAnimationTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Weapon Test")]
    [SerializeField] private GameObject testWeapon;

    [Header("Weapon Position")]
    [SerializeField] private Vector3 weaponPositionOffset;

    [Header("Weapon Rotation")]
    [SerializeField] private Vector3 weaponRotationOffset;

    private Transform rightHand;

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");


    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }


    private void Start()
    {
        FindRightHand();

        if (testWeapon != null)
        {
            AttachWeapon();
        }
    }


    private void Update()
    {
        if (animator == null)
        {
            return;
        }

        UpdateWeaponPosition();

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animator.SetTrigger(AttackHash);
        }
    }


    private void FindRightHand()
    {
        if (animator == null)
        {
            Debug.LogError(
                "CombatAnimationTest has no Animator.",
                gameObject
            );

            return;
        }

        rightHand =
            animator.GetBoneTransform(
                HumanBodyBones.RightHand
            );

        if (rightHand == null)
        {
            Debug.LogError(
                "Could not find humanoid RightHand bone.",
                gameObject
            );

            return;
        }

        Debug.Log(
            $"RIGHT HAND FOUND: {rightHand.name}",
            rightHand
        );
    }


    private void AttachWeapon()
    {
        if (rightHand == null ||
            testWeapon == null)
        {
            return;
        }

        testWeapon.transform.SetParent(
            rightHand,
            false
        );

        UpdateWeaponPosition();
    }


    private void UpdateWeaponPosition()
    {
        if (testWeapon == null ||
            rightHand == null)
        {
            return;
        }

        testWeapon.transform.localPosition =
            weaponPositionOffset;

        testWeapon.transform.localRotation =
            Quaternion.Euler(
                weaponRotationOffset
            );
    }
}