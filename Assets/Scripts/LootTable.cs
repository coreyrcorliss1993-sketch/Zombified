using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewLootTable",
    menuName = "Loot System/Loot Table"
)]
public class LootTable : ScriptableObject
{
    [Serializable]
    public class LootEntry
    {
        [SerializeField] private ItemDefinition item;

        [Min(0f)]
        [SerializeField] private float weight = 1f;

        public ItemDefinition Item => item;
        public float Weight => Mathf.Max(0f, weight);
    }

    [Header("Weighted Items")]
    [SerializeField]
    private List<LootEntry> entries =
        new List<LootEntry>();

    public IReadOnlyList<LootEntry> Entries => entries;


    public ItemDefinition RollItem()
    {
        float totalWeight = CalculateTotalWeight();

        if (totalWeight <= 0f)
        {
            Debug.LogWarning(
                $"Loot table '{name}' has no positive item weights.",
                this
            );

            return null;
        }

        float roll =
            UnityEngine.Random.Range(
                0f,
                totalWeight
            );

        float cumulativeWeight = 0f;

        foreach (LootEntry entry in entries)
        {
            if (entry == null ||
                entry.Item == null ||
                entry.Weight <= 0f)
            {
                continue;
            }

            cumulativeWeight += entry.Weight;

            if (roll <= cumulativeWeight)
            {
                return entry.Item;
            }
        }

        // Floating-point safety fallback.
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            LootEntry entry = entries[i];

            if (entry != null &&
                entry.Item != null &&
                entry.Weight > 0f)
            {
                return entry.Item;
            }
        }

        return null;
    }


    public float CalculateTotalWeight()
    {
        float totalWeight = 0f;

        foreach (LootEntry entry in entries)
        {
            if (entry == null ||
                entry.Item == null)
            {
                continue;
            }

            totalWeight += entry.Weight;
        }

        return totalWeight;
    }


    public float GetChancePercent(LootEntry targetEntry)
    {
        if (targetEntry == null ||
            targetEntry.Item == null ||
            targetEntry.Weight <= 0f)
        {
            return 0f;
        }

        float totalWeight = CalculateTotalWeight();

        if (totalWeight <= 0f)
        {
            return 0f;
        }

        return
            targetEntry.Weight /
            totalWeight *
            100f;
    }
}