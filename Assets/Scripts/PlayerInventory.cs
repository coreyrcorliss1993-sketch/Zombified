using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [Min(1)]
    [SerializeField] private int maxSlots = 12;

    [SerializeField]
    private List<InventoryItem> items =
        new List<InventoryItem>();


    public IReadOnlyList<InventoryItem> Items =>
        items;

    public int MaxSlots =>
        maxSlots;

    public int UsedSlots =>
        items.Count;


    public bool AddItem(
        ItemDefinition itemDefinition,
        int quantity = 1
    )
    {
        if (itemDefinition == null)
        {
            Debug.LogError(
                "PlayerInventory received a missing ItemDefinition.",
                gameObject
            );

            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        if (!CanAddItem(itemDefinition, quantity))
        {
            Debug.Log(
                $"Inventory full. Could not add " +
                $"{quantity}x {itemDefinition.DisplayName}."
            );

            return false;
        }

        if (itemDefinition.Stackable)
        {
            AddStackableItem(
                itemDefinition,
                quantity
            );
        }
        else
        {
            AddNonStackableItem(
                itemDefinition,
                quantity
            );
        }

        Debug.Log(
            $"Added {quantity}x " +
            $"{itemDefinition.DisplayName}."
        );

        PrintInventory();

        return true;
    }


    public bool CanAddItem(
        ItemDefinition itemDefinition,
        int quantity = 1
    )
    {
        if (itemDefinition == null ||
            quantity <= 0)
        {
            return false;
        }

        if (!itemDefinition.Stackable)
        {
            int freeSlots =
                maxSlots - items.Count;

            return freeSlots >= quantity;
        }

        int remainingQuantity = quantity;

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem inventoryItem = items[i];

            if (!IsSameItem(
                    inventoryItem,
                    itemDefinition
                ))
            {
                continue;
            }

            int freeSpace =
                itemDefinition.MaximumStackSize -
                inventoryItem.quantity;

            if (freeSpace <= 0)
            {
                continue;
            }

            remainingQuantity -= freeSpace;

            if (remainingQuantity <= 0)
            {
                return true;
            }
        }

        int requiredNewStacks =
            Mathf.CeilToInt(
                remainingQuantity /
                (float)itemDefinition.MaximumStackSize
            );

        int availableSlots =
            maxSlots - items.Count;

        return availableSlots >= requiredNewStacks;
    }


    private void AddStackableItem(
        ItemDefinition itemDefinition,
        int quantity
    )
    {
        int remainingQuantity = quantity;

        // Fill existing partial stacks first.
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem inventoryItem = items[i];

            if (!IsSameItem(
                    inventoryItem,
                    itemDefinition
                ))
            {
                continue;
            }

            int freeSpace =
                itemDefinition.MaximumStackSize -
                inventoryItem.quantity;

            if (freeSpace <= 0)
            {
                continue;
            }

            int amountToAdd =
                Mathf.Min(
                    freeSpace,
                    remainingQuantity
                );

            inventoryItem.quantity += amountToAdd;
            remainingQuantity -= amountToAdd;

            if (remainingQuantity <= 0)
            {
                return;
            }
        }

        // Create new stacks if necessary.
        while (remainingQuantity > 0)
        {
            int stackQuantity =
                Mathf.Min(
                    remainingQuantity,
                    itemDefinition.MaximumStackSize
                );

            items.Add(
                new InventoryItem(
                    itemDefinition,
                    stackQuantity
                )
            );

            remainingQuantity -= stackQuantity;
        }
    }


    private void AddNonStackableItem(
        ItemDefinition itemDefinition,
        int quantity
    )
    {
        for (int i = 0; i < quantity; i++)
        {
            items.Add(
                new InventoryItem(
                    itemDefinition,
                    1
                )
            );
        }
    }


    public bool RemoveItem(
        ItemDefinition itemDefinition,
        int quantity = 1
    )
    {
        if (itemDefinition == null ||
            quantity <= 0)
        {
            return false;
        }

        if (!HasItem(itemDefinition, quantity))
        {
            return false;
        }

        int remainingToRemove = quantity;

        for (int i = items.Count - 1;
             i >= 0;
             i--)
        {
            InventoryItem inventoryItem = items[i];

            if (!IsSameItem(
                    inventoryItem,
                    itemDefinition
                ))
            {
                continue;
            }

            int removedFromStack =
                Mathf.Min(
                    inventoryItem.quantity,
                    remainingToRemove
                );

            inventoryItem.quantity -= removedFromStack;
            remainingToRemove -= removedFromStack;

            if (inventoryItem.quantity <= 0)
            {
                items.RemoveAt(i);
            }

            if (remainingToRemove <= 0)
            {
                break;
            }
        }

        PrintInventory();

        return true;
    }


    public bool HasItem(
        ItemDefinition itemDefinition,
        int quantity = 1
    )
    {
        if (itemDefinition == null ||
            quantity <= 0)
        {
            return false;
        }

        int totalQuantity = 0;

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem inventoryItem = items[i];

            if (!IsSameItem(
                    inventoryItem,
                    itemDefinition
                ))
            {
                continue;
            }

            totalQuantity += inventoryItem.quantity;

            if (totalQuantity >= quantity)
            {
                return true;
            }
        }

        return false;
    }


    private bool IsSameItem(
        InventoryItem inventoryItem,
        ItemDefinition itemDefinition
    )
    {
        return
            inventoryItem != null &&
            inventoryItem.ItemDefinition ==
            itemDefinition;
    }

    public bool CanAddItems(
    IReadOnlyList<ItemDefinition> itemDefinitions
)
    {
        if (itemDefinitions == null)
        {
            return false;
        }

        int simulatedSlots = items.Count;

        Dictionary<ItemDefinition, int>
            simulatedQuantities =
                new Dictionary<ItemDefinition, int>();

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem inventoryItem = items[i];

            if (inventoryItem == null ||
                inventoryItem.ItemDefinition == null)
            {
                continue;
            }

            ItemDefinition definition =
                inventoryItem.ItemDefinition;

            if (!simulatedQuantities.ContainsKey(definition))
            {
                simulatedQuantities[definition] = 0;
            }

            simulatedQuantities[definition] +=
                inventoryItem.quantity;
        }

        for (int i = 0;
             i < itemDefinitions.Count;
             i++)
        {
            ItemDefinition definition =
                itemDefinitions[i];

            if (definition == null)
            {
                continue;
            }

            if (!definition.Stackable)
            {
                simulatedSlots++;

                if (simulatedSlots > maxSlots)
                {
                    return false;
                }

                continue;
            }

            int existingQuantity = 0;

            simulatedQuantities.TryGetValue(
                definition,
                out existingQuantity
            );

            int existingStacks = 0;

            for (int j = 0; j < items.Count; j++)
            {
                if (items[j] != null &&
                    items[j].ItemDefinition == definition)
                {
                    existingStacks++;
                }
            }

            int newQuantity =
                existingQuantity + 1;

            int requiredStacks =
                Mathf.CeilToInt(
                    newQuantity /
                    (float)definition.MaximumStackSize
                );

            if (requiredStacks > existingStacks)
            {
                simulatedSlots +=
                    requiredStacks - existingStacks;
            }

            if (simulatedSlots > maxSlots)
            {
                return false;
            }

            simulatedQuantities[definition] =
                newQuantity;
        }

        return true;
    }

    public void PrintInventory()
    {
        Debug.Log(
            $"INVENTORY: {items.Count}/" +
            $"{maxSlots} slots used"
        );

        if (items.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];

            Debug.Log(
                $"{item.itemName} x{item.quantity}"
            );
        }
    }
}