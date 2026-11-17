using System.Collections.Generic;
using UnityEngine;

public class SearchableCrate : MonoBehaviour
{
    [Header("Search Settings")]
    [Min(0.1f)]
    [SerializeField]
    private float searchDuration = 3f;

    [Header("Loot Tables")]
    [SerializeField]
    private ContainerRollTable containerRollTable;

    [SerializeField]
    private LootTable lootTable;

    private bool hasBeenSearched;


    public float SearchDuration =>
        searchDuration;

    public bool HasBeenSearched =>
        hasBeenSearched;


    public bool CompleteSearch(
        PlayerInventory inventory
    )
    {
        if (hasBeenSearched)
        {
            return false;
        }

        if (inventory == null)
        {
            Debug.LogError(
                "SearchableCrate received no inventory.",
                gameObject
            );

            return false;
        }

        if (containerRollTable == null)
        {
            Debug.LogError(
                $"{name} has no Container Roll Table.",
                gameObject
            );

            return false;
        }

        if (lootTable == null)
        {
            Debug.LogError(
                $"{name} has no Loot Table.",
                gameObject
            );

            return false;
        }

        int numberOfRolls =
            containerRollTable.RollNumberOfItems();

        // An empty container is still considered searched.
        if (numberOfRolls <= 0)
        {
            hasBeenSearched = true;

            Debug.Log(
                $"{name.ToUpper()} SEARCHED: " +
                "Nothing useful found."
            );

            return true;
        }

        List<ItemDefinition> rolledItems =
            RollItems(numberOfRolls);

        if (rolledItems.Count <= 0)
        {
            hasBeenSearched = true;

            Debug.Log(
                $"{name.ToUpper()} SEARCHED: " +
                "Nothing useful found."
            );

            return true;
        }

        if (!inventory.CanAddItems(rolledItems))

        {
            Debug.Log(
                $"Not enough inventory space to loot {name}."
            );

            return false;
        }

        AddAllLoot(
            inventory,
            rolledItems
        );

        hasBeenSearched = true;

        Debug.Log(
            $"{name.ToUpper()} SEARCHED: " +
            $"Received {rolledItems.Count} loot rolls."
        );

        return true;
    }


    private List<ItemDefinition> RollItems(
        int numberOfRolls
    )
    {
        List<ItemDefinition> rolledItems =
            new List<ItemDefinition>();

        for (int i = 0;
             i < numberOfRolls;
             i++)
        {
            ItemDefinition rolledItem =
                lootTable.RollItem();

            if (rolledItem == null)
            {
                continue;
            }

            rolledItems.Add(rolledItem);
        }

        return rolledItems;
    }


    private bool CanFitAllLoot(
        PlayerInventory inventory,
        List<ItemDefinition> rolledItems
    )
    {
        Dictionary<ItemDefinition, int> totals =
            new Dictionary<ItemDefinition, int>();

        for (int i = 0;
             i < rolledItems.Count;
             i++)
        {
            ItemDefinition item =
                rolledItems[i];

            if (item == null)
            {
                continue;
            }

            if (!totals.ContainsKey(item))
            {
                totals[item] = 0;
            }

            totals[item]++;
        }

        foreach (
            KeyValuePair<ItemDefinition, int> pair
            in totals
        )
        {
            if (!inventory.CanAddItem(
                    pair.Key,
                    pair.Value
                ))
            {
                return false;
            }
        }

        return true;
    }


    private void AddAllLoot(
        PlayerInventory inventory,
        List<ItemDefinition> rolledItems
    )
    {
        Dictionary<ItemDefinition, int> totals =
            new Dictionary<ItemDefinition, int>();

        for (int i = 0;
             i < rolledItems.Count;
             i++)
        {
            ItemDefinition item =
                rolledItems[i];

            if (item == null)
            {
                continue;
            }

            if (!totals.ContainsKey(item))
            {
                totals[item] = 0;
            }

            totals[item]++;
        }

        foreach (
            KeyValuePair<ItemDefinition, int> pair
            in totals
        )
        {
            inventory.AddItem(
                pair.Key,
                pair.Value
            );

            Debug.Log(
                $"FOUND: {pair.Value}x " +
                $"{pair.Key.DisplayName}"
            );
        }
    }
}