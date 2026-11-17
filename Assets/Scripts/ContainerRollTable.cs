using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DefaultContainerRollTable",
    menuName = "Loot System/Container Roll Table"
)]
public class ContainerRollTable : ScriptableObject
{
    [Serializable]
    public class RollOutcome
    {
        [Min(0)]
        [SerializeField] private int numberOfLootRolls;

        [Min(0f)]
        [SerializeField] private float weight = 1f;

        public int NumberOfLootRolls =>
            Mathf.Max(0, numberOfLootRolls);

        public float Weight =>
            Mathf.Max(0f, weight);
    }

    [Header("Container Fullness Outcomes")]
    [SerializeField]
    private List<RollOutcome> outcomes =
        new List<RollOutcome>();


    public int RollNumberOfItems()
    {
        float totalWeight = CalculateTotalWeight();

        if (totalWeight <= 0f)
        {
            Debug.LogWarning(
                $"Container roll table '{name}' " +
                "has no positive weights.",
                this
            );

            return 0;
        }

        float roll =
            UnityEngine.Random.Range(
                0f,
                totalWeight
            );

        float cumulativeWeight = 0f;

        foreach (RollOutcome outcome in outcomes)
        {
            if (outcome == null ||
                outcome.Weight <= 0f)
            {
                continue;
            }

            cumulativeWeight += outcome.Weight;

            if (roll <= cumulativeWeight)
            {
                return outcome.NumberOfLootRolls;
            }
        }

        // Floating-point safety fallback.
        for (int i = outcomes.Count - 1; i >= 0; i--)
        {
            RollOutcome outcome = outcomes[i];

            if (outcome != null &&
                outcome.Weight > 0f)
            {
                return outcome.NumberOfLootRolls;
            }
        }

        return 0;
    }


    public float CalculateTotalWeight()
    {
        float totalWeight = 0f;

        foreach (RollOutcome outcome in outcomes)
        {
            if (outcome == null)
            {
                continue;
            }

            totalWeight += outcome.Weight;
        }

        return totalWeight;
    }
}