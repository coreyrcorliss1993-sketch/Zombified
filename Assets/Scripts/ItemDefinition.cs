using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "Loot System/Item Definition"
)]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemID;
    [SerializeField] private string displayName;

    [TextArea(2, 5)]
    [SerializeField] private string description;

    [Header("Inventory")]
    [SerializeField] private bool stackable = true;

    [Min(1)]
    [SerializeField] private int maximumStackSize = 50;

    [Header("Presentation")]
    [SerializeField] private Sprite inventoryIcon;

    public string ItemID => itemID;
    public string DisplayName => displayName;
    public string Description => description;
    public bool Stackable => stackable;

    public int MaximumStackSize
    {
        get
        {
            if (!stackable)
            {
                return 1;
            }

            return Mathf.Max(1, maximumStackSize);
        }
    }

    public Sprite InventoryIcon => inventoryIcon;


    private void OnValidate()
    {
        if (!stackable)
        {
            maximumStackSize = 1;
        }
        else
        {
            maximumStackSize =
                Mathf.Max(1, maximumStackSize);
        }
    }
}