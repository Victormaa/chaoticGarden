using UnityEditor.ShaderGraph;
using UnityEngine;

/// <summary>
/// Seed data
/// </summary>
[CreateAssetMenu(fileName = "NewSeed", menuName = "Farming/Seed Data")]
public class SeedData : ItemData
{
    [Header("Planting Information")]
    [Tooltip("Grown crops")]
    public CropData cropToGrow;

    [Tooltip("Growth time (seconds)")]
    public float growthTime = 10f;

    [Tooltip("Required level for planting")]
    public int requiredLevel = 1;

    [Header("Output")]
    [Tooltip("Quantity of crops produced at harvest time")]
    public int harvestYield = 1;

    [Tooltip("The number of seeds obtained after harvest")]
    public int seedsOnHarvest = 0;

    void OnValidate()
    {
        itemType = ItemType.Seed;
    }
}
