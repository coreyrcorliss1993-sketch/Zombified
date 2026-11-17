using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private PlayerInventory playerInventory;

    [Header("UI")]
    [SerializeField]
    private GameObject inventoryPanel;

    [Header("Inventory Slots")]
    [SerializeField]
    private InventorySlotUI[] inventorySlots;

    private PlayerInput playerInput;
    private InputAction inventoryAction;

    private bool inventoryOpen;


    private void Awake()
    {
        playerInput =
            GetComponent<PlayerInput>();

        inventoryAction =
            playerInput.actions.FindAction(
                "Inventory"
            );

        if (inventoryAction == null)
        {
            Debug.LogError(
                "PlayerInventoryUI could not find " +
                "the Inventory action.",
                gameObject
            );
        }
    }


    private void Start()
    {
        inventoryOpen = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        ClearAllSlots();
    }


    private void Update()
    {
        if (inventoryAction == null)
        {
            return;
        }

        if (inventoryAction.WasPressedThisFrame())
        {
            ToggleInventory();
        }

        if (inventoryOpen)
        {
            RefreshInventory();
        }
    }


    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(
                inventoryOpen
            );
        }

        if (inventoryOpen)
        {
            RefreshInventory();
        }
    }


    private void RefreshInventory()
    {
        if (playerInventory == null)
        {
            return;
        }

        if (inventorySlots == null)
        {
            return;
        }

        for (int i = 0;
             i < inventorySlots.Length;
             i++)
        {
            InventorySlotUI slot =
                inventorySlots[i];

            if (slot == null)
            {
                continue;
            }

            if (i < playerInventory.Items.Count)
            {
                slot.DisplayItem(
                    playerInventory.Items[i]
                );
            }
            else
            {
                slot.ClearSlot();
            }
        }
    }


    private void ClearAllSlots()
    {
        if (inventorySlots == null)
        {
            return;
        }

        for (int i = 0;
             i < inventorySlots.Length;
             i++)
        {
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }
}