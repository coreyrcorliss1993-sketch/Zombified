using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int currentAmmo = 30;
    [SerializeField] private int reserveAmmo = 90;

    [Header("Fire Settings")]
    [SerializeField] private float fireRate = 0.12f;
    [SerializeField] private bool automatic = true;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 2f;

    private bool isReloading;
    private bool attackHeld;
    private float nextFireTime;

    private static readonly int ShootHash =
        Animator.StringToHash("Shoot");

    private static readonly int ReloadHash =
        Animator.StringToHash("Reload");

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        currentAmmo = Mathf.Clamp(
            currentAmmo,
            0,
            magazineSize
        );
    }

    private void Update()
    {
        if (automatic &&
            attackHeld &&
            !isReloading &&
            Time.time >= nextFireTime)
        {
            TryShoot();
        }
    }

    // Called by PlayerInput's existing "Attack" action.
    public void OnAttack(InputValue value)
    {
        attackHeld = value.isPressed;

        if (value.isPressed && !automatic)
        {
            TryShoot();
        }
    }

    // We'll connect an Input Action named "Reload" to this.
    public void OnReload(InputValue value)
    {
        if (!value.isPressed)
            return;

        TryReload();
    }

    private void TryShoot()
    {
        if (isReloading)
            return;

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            TryReload();
            return;
        }

        currentAmmo--;

        nextFireTime = Time.time + fireRate;

        if (animator != null)
        {
            animator.SetTrigger(ShootHash);
        }

        Debug.Log(
            $"M16 fired. Ammo: {currentAmmo}/{reserveAmmo}"
        );

        // Actual hit detection will go here next.
    }

    private void TryReload()
    {
        if (isReloading)
            return;

        if (currentAmmo >= magazineSize)
            return;

        if (reserveAmmo <= 0)
            return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        attackHeld = false;

        if (animator != null)
        {
            animator.SetTrigger(ReloadHash);
        }

        Debug.Log("Reload started.");

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;

        Debug.Log(
            $"Reload complete. Ammo: {currentAmmo}/{reserveAmmo}"
        );
    }
}