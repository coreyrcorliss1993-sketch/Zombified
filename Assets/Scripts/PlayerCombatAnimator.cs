using UnityEngine;

public class PlayerCombatAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private PlayerMelee playerMelee;
    [SerializeField] private GameObject equippedWeapon;

    [Header("Weapon Position")]
    [SerializeField]
    private Vector3 weaponPositionOffset =
        new Vector3(0.03f, 0.15f, 0.03f);

    [Header("Weapon Rotation")]
    [SerializeField]
    private Vector3 weaponRotationOffset =
        new Vector3(0f, -12f, 90f);

    private Transform rightHand;

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");


    private void Awake()
    {
        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
        }
    }


    private void Start()
    {
        FindRightHand();
        AttachWeapon();
    }


    private void FindRightHand()
    {
        if (characterAnimator == null)
        {
            Debug.LogError(
                "PlayerCombatAnimator has no Animator assigned.",
                gameObject
            );

            return;
        }

        if (characterAnimator.avatar == null ||
            !characterAnimator.avatar.isValid ||
            !characterAnimator.avatar.isHuman)
        {
            Debug.LogError(
                "PlayerCombatAnimator requires a valid Humanoid Avatar.",
                gameObject
            );

            return;
        }

        rightHand =
            characterAnimator.GetBoneTransform(
                HumanBodyBones.RightHand
            );

        if (rightHand == null)
        {
            Debug.LogError(
                "PlayerCombatAnimator could not find the RightHand bone.",
                gameObject
            );

            return;
        }

        Debug.Log(
            $"PLAYER RIGHT HAND FOUND: {rightHand.name}",
            rightHand
        );
    }


    private void AttachWeapon()
    {
        if (rightHand == null)
        {
            Debug.LogWarning(
                "Cannot attach weapon because RightHand was not found.",
                gameObject
            );

            return;
        }

        if (equippedWeapon == null)
        {
            Debug.LogWarning(
                "PlayerCombatAnimator has no equipped weapon.",
                gameObject
            );

            return;
        }

        equippedWeapon.transform.SetParent(
            rightHand,
            false
        );

        equippedWeapon.transform.localPosition =
            weaponPositionOffset;

        equippedWeapon.transform.localRotation =
            Quaternion.Euler(
                weaponRotationOffset
            );
    }


    public void PlayAttack()
    {
        if (characterAnimator == null)
        {
            return;
        }

        characterAnimator.SetTrigger(AttackHash);
    }


    // Called by the Animation Event
    // at the exact weapon impact frame.
    public void AnimationMeleeImpact()
    {
        if (playerMelee == null)
        {
            Debug.LogError(
                "AnimationMeleeImpact fired, but PlayerMelee is not assigned.",
                gameObject
            );

            return;
        }

        playerMelee.MeleeImpact();
    }


    // Called by the Animation Event
    // near the end of the attack animation.
    public void AnimationMeleeFinished()
    {
        if (playerMelee == null)
        {
            Debug.LogError(
                "AnimationMeleeFinished fired, but PlayerMelee is not assigned.",
                gameObject
            );

            return;
        }

        playerMelee.MeleeAttackFinished();
    }
}