using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatMaster : MonoBehaviour
{
    public enum CombatMode { Unarmed, Melee, Rifle }

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform muzzlePoint;

    [Header("Equipped Mode")]
    [SerializeField] private CombatMode combatMode = CombatMode.Rifle;

    [Header("Rifle")]
    [SerializeField] private bool automatic = true;
    [SerializeField] private float rifleDamage = 25f;
    [SerializeField] private float rifleRange = 100f;
    [SerializeField] private float fireInterval = 0.12f;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int currentAmmo = 30;
    [SerializeField] private int reserveAmmo = 90;
    [SerializeField] private float reloadDuration = 2f;
    [SerializeField] private LayerMask rifleHitMask = ~0;

    [Header("Melee")]
    [SerializeField] private Transform meleeOrigin;
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float meleeRadius = 1.2f;
    [SerializeField] private float meleeCooldown = 0.65f;
    [SerializeField] private LayerMask meleeHitMask = ~0;

    private bool attackHeld;
    private bool isReloading;
    private float nextAttackTime;

    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");
    private static readonly int MeleeHash = Animator.StringToHash("Melee");

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;
    public CombatMode CurrentMode => combatMode;

    private void Awake()
    {
        currentAmmo = Mathf.Clamp(currentAmmo, 0, magazineSize);
        if (aimCamera == null) aimCamera = Camera.main;
    }

    private void Update()
    {
        if (combatMode == CombatMode.Rifle && automatic && attackHeld)
            TryAttack();
    }

    public void OnAttack(InputValue value)
    {
        attackHeld = value.isPressed;
        if (value.isPressed && (combatMode != CombatMode.Rifle || !automatic))
            TryAttack();
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed && combatMode == CombatMode.Rifle)
            TryReload();
    }

    public void SetCombatMode(CombatMode newMode)
    {
        attackHeld = false;
        combatMode = newMode;
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime || isReloading) return;

        if (combatMode == CombatMode.Rifle)
            FireRifle();
        else
            PerformMelee();
    }

    private void FireRifle()
    {
        if (currentAmmo <= 0)
        {
            TryReload();
            return;
        }

        currentAmmo--;
        nextAttackTime = Time.time + fireInterval;
        SetTriggerIfPresent(ShootHash);

        Vector3 origin;
        Vector3 direction;
        if (aimCamera != null)
        {
            origin = aimCamera.transform.position;
            direction = aimCamera.transform.forward;
        }
        else if (muzzlePoint != null)
        {
            origin = muzzlePoint.position;
            direction = muzzlePoint.forward;
        }
        else
        {
            origin = transform.position + Vector3.up;
            direction = transform.forward;
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rifleRange, rifleHitMask, QueryTriggerInteraction.Ignore))
            hit.collider.SendMessage("TakeDamage", rifleDamage, SendMessageOptions.DontRequireReceiver);

        Debug.DrawRay(origin, direction * rifleRange, Color.red, 0.35f);
    }

    private void PerformMelee()
    {
        nextAttackTime = Time.time + meleeCooldown;
        SetTriggerIfPresent(MeleeHash);

        Transform origin = meleeOrigin != null ? meleeOrigin : transform;
        Collider[] hits = Physics.OverlapSphere(origin.position, meleeRadius, meleeHitMask, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            if (hit.transform.root == transform.root) continue;
            hit.SendMessage("TakeDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void TryReload()
    {
        if (isReloading || currentAmmo >= magazineSize || reserveAmmo <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        attackHeld = false;
        SetTriggerIfPresent(ReloadHash);

        yield return new WaitForSeconds(reloadDuration);

        int required = magazineSize - currentAmmo;
        int loaded = Mathf.Min(required, reserveAmmo);
        currentAmmo += loaded;
        reserveAmmo -= loaded;
        isReloading = false;
    }

    private void SetTriggerIfPresent(int hash)
    {
        if (animator == null) return;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == hash && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(hash);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = meleeOrigin != null ? meleeOrigin : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin.position, meleeRadius);
    }
}
