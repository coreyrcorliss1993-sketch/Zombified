using UnityEngine;

public class PlayerFirstPersonBody : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Transform cameraHead;

    [Header("Camera Placement")]
    [SerializeField]
    private Vector3 cameraOffset =
        new Vector3(0f, -0.05f, 0.20f);

    [Header("Follow Settings")]
    [SerializeField] private bool followAnimatedHead = true;

    private Transform humanoidHead;


    private void Start()
    {
        FindHumanoidHead();

        if (humanoidHead != null)
        {
            UpdateCameraPosition();
        }
    }


    private void LateUpdate()
    {
        if (!followAnimatedHead)
        {
            return;
        }

        if (humanoidHead == null ||
            cameraHead == null)
        {
            return;
        }

        UpdateCameraPosition();
    }


    private void FindHumanoidHead()
    {
        if (characterAnimator == null)
        {
            Debug.LogError(
                "PlayerFirstPersonBody has no Character Animator.",
                gameObject
            );

            return;
        }

        humanoidHead =
            characterAnimator.GetBoneTransform(
                HumanBodyBones.Head
            );

        if (humanoidHead == null)
        {
            Debug.LogError(
                "Could not find humanoid Head bone.",
                gameObject
            );

            return;
        }

        Debug.Log(
            $"PLAYER HEAD FOUND: {humanoidHead.name}",
            humanoidHead
        );
    }


    private void UpdateCameraPosition()
    {
        // Get the animated humanoid head position
        // relative to the Player root.
        Vector3 localHeadPosition =
            transform.InverseTransformPoint(
                humanoidHead.position
            );

        cameraHead.localPosition =
            localHeadPosition +
            cameraOffset;
    }
}