using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField]
    private ItemDefinition itemDefinition;

    [Min(1)]
    public int quantity = 1;


    public ItemDefinition ItemDefinition =>
        itemDefinition;

    public string itemName
    {
        get
        {
            if (itemDefinition == null)
            {
                return "Missing Item";
            }

            return itemDefinition.DisplayName;
        }
    }

    public bool IsStackable =>
        itemDefinition != null &&
        itemDefinition.Stackable;

    public int MaximumStackSize
    {
        get
        {
            if (itemDefinition == null)
            {
                return 1;
            }

            return itemDefinition.MaximumStackSize;
        }
    }


    public InventoryItem(
        ItemDefinition itemDefinition,
        int quantity
    )
    {
        this.itemDefinition = itemDefinition;

        if (itemDefinition == null)
        {
            this.quantity = 1;
            return;
        }

        if (!itemDefinition.Stackable)
        {
            this.quantity = 1;
            return;
        }

        this.quantity =
            Mathf.Clamp(
                quantity,
                1,
                itemDefinition.MaximumStackSize
            );
    }
}