using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3f;

    private PlayerInput playerInput;
    private InputAction interactAction;
    private PlayerInventory playerInventory;

    private SearchableCrate currentCrate;

    private float searchProgress;
    private bool isSearching;


    public bool IsSearching => isSearching;

    public bool CanSearch
    {
        get
        {
            return currentCrate != null &&
                   !currentCrate.HasBeenSearched;
        }
    }


    public float SearchProgressNormalized
    {
        get
        {
            if (currentCrate == null)
            {
                return 0f;
            }

            if (currentCrate.SearchDuration <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(
                searchProgress /
                currentCrate.SearchDuration
            );
        }
    }


    private void Awake()
    {
        playerInput =
            GetComponent<PlayerInput>();

        playerInventory =
            GetComponent<PlayerInventory>();

        interactAction =
            playerInput.actions.FindAction(
                "Interact"
            );

        if (interactAction == null)
        {
            Debug.LogError(
                "PlayerInteraction could not find " +
                "the Interact action.",
                gameObject
            );
        }

        if (playerInventory == null)
        {
            Debug.LogError(
                "PlayerInteraction could not find " +
                "PlayerInventory.",
                gameObject
            );
        }
    }


    private void Update()
    {
        if (interactAction == null)
        {
            return;
        }

        SearchableCrate lookedAtCrate =
            FindCrateInFrontOfPlayer();

        HandleCrateTarget(lookedAtCrate);
        HandleSearch();
    }


    private SearchableCrate FindCrateInFrontOfPlayer()
    {
        if (playerCamera == null)
        {
            return null;
        }

        Vector3 origin =
            playerCamera.transform.position;

        Vector3 direction =
            playerCamera.transform.forward;

        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            interactionRange,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        ))
        {
            return hit.collider
                .GetComponentInParent<SearchableCrate>();
        }

        return null;
    }


    private void HandleCrateTarget(
        SearchableCrate lookedAtCrate
    )
    {
        if (lookedAtCrate == currentCrate)
        {
            return;
        }

        // Looking away from the container cancels
        // any search already in progress.
        if (isSearching)
        {
            CancelSearch();
        }

        currentCrate = lookedAtCrate;
        searchProgress = 0f;

        if (currentCrate != null &&
            !currentCrate.HasBeenSearched)
        {
            Debug.Log(
                $"Searchable container detected: " +
                $"{currentCrate.gameObject.name}"
            );
        }
    }


    private void HandleSearch()
    {
        if (currentCrate == null)
        {
            return;
        }

        if (currentCrate.HasBeenSearched)
        {
            if (isSearching)
            {
                CancelSearch();
            }

            return;
        }

        // Player must HOLD the Interact action.
        if (interactAction.IsPressed())
        {
            if (!isSearching)
            {
                BeginSearch();
            }

            searchProgress += Time.deltaTime;

            int searchPercent =
            Mathf.RoundToInt(
        SearchProgressNormalized * 100f
     );

            Debug.Log(
                "Searching... " +
                searchPercent +
                "%"
            );

            if (searchProgress >=
                currentCrate.SearchDuration)
            {
                FinishSearch();
            }
        }
        else if (isSearching)
        {
            CancelSearch();
        }
    }


    private void BeginSearch()
    {
        isSearching = true;
        searchProgress = 0f;

        Debug.Log("SEARCH STARTED");
    }


    private void FinishSearch()
    {
        if (currentCrate == null)
        {
            return;
        }

        searchProgress = currentCrate.SearchDuration;

        bool searchCompleted =
            currentCrate.CompleteSearch(playerInventory);

        if (searchCompleted)
        {
            Debug.Log(
                "SEARCH COMPLETE: " +
                currentCrate.gameObject.name
            );
        }
        else
        {
            Debug.Log(
                "SEARCH COULD NOT COMPLETE: " +
                currentCrate.gameObject.name
            );
        }

        isSearching = false;
        searchProgress = 0f;
    }


    private void CancelSearch()
    {
        isSearching = false;
        searchProgress = 0f;

        Debug.Log("SEARCH CANCELLED");
    }
}