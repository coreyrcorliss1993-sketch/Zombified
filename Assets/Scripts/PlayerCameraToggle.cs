using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerCameraToggle : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform playerCamera;

    [Header("Survivalist")]
    [Tooltip("Drag Survivalist (2) here.")]
    [SerializeField] private GameObject survivalistRoot;

    [Tooltip("Drag FPS_HANDS here.")]
    [SerializeField] private GameObject fpsHands;

    [Header("First Person")]
    [SerializeField] private Vector3 firstPersonPosition = Vector3.zero;

    [Header("Third Person")]
    [SerializeField]
    private Vector3 thirdPersonPosition =
        new Vector3(0f, 0.5f, -4f);

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 10f;

    private bool isThirdPerson = false;
    private Vector3 targetPosition;

    private Renderer[] bodyRenderers;
    private Renderer[] fpsRenderers;

    private void Start()
    {
        if (playerCamera != null)
        {
            firstPersonPosition = playerCamera.localPosition;
            targetPosition = firstPersonPosition;
        }

        FindRenderers();
        SetCameraMode(false);
    }

    private void Update()
    {
        if (playerCamera == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.vKey.wasPressedThisFrame)
        {
            isThirdPerson = !isThirdPerson;
            SetCameraMode(isThirdPerson);
        }

        playerCamera.localPosition = Vector3.Lerp(
            playerCamera.localPosition,
            targetPosition,
            transitionSpeed * Time.deltaTime
        );
    }

    private void FindRenderers()
    {
        // Find FPS hand renderers.
        if (fpsHands != null)
        {
            fpsRenderers = fpsHands.GetComponentsInChildren<Renderer>(true);
        }

        // Find all Survivalist renderers except FPS_HANDS.
        if (survivalistRoot != null)
        {
            Renderer[] allRenderers =
                survivalistRoot.GetComponentsInChildren<Renderer>(true);

            List<Renderer> normalBodyRenderers =
                new List<Renderer>();

            foreach (Renderer renderer in allRenderers)
            {
                if (fpsHands != null &&
                    renderer.transform.IsChildOf(fpsHands.transform))
                {
                    continue;
                }

                normalBodyRenderers.Add(renderer);
            }

            bodyRenderers = normalBodyRenderers.ToArray();
        }
    }

    private void SetCameraMode(bool thirdPerson)
    {
        targetPosition = thirdPerson
            ? thirdPersonPosition
            : firstPersonPosition;

        // Normal body:
        // OFF in first person
        // ON in third person
        SetRenderers(bodyRenderers, thirdPerson);

        // FPS hands:
        // ON in first person
        // OFF in third person
        SetRenderers(fpsRenderers, !thirdPerson);
    }

    private void SetRenderers(Renderer[] renderers, bool visible)
    {
        if (renderers == null)
            return;

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }
}