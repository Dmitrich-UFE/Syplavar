using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class RecipeData : ScriptableObject
{
    // ===== INGREDIENTS =====
    [SerializeField] private ItemData[] items;
    [SerializeField] private int[] counts;

    // ===== RESULT =====
    [SerializeField] private ItemData resultItem;
    [SerializeField] private int resultCount;

    // ===== ID =====
    [SerializeField] private int craftID;

    // ===== PROPERTIES =====

    internal int CraftID => craftID;

    internal ItemData ResultItem => resultItem;

    internal int ResultCount => resultCount;

    internal int IngredientsCount => items != null ? items.Length : 0;

    // ===== INDEXER =====
    internal (ItemData item, int count) this[int index]
    {
        get
        {
            if (items == null || counts == null)
                throw new System.Exception("Массивы не инициализированы");

            if (index < 0 || index >= items.Length || index >= counts.Length)
                throw new System.IndexOutOfRangeException();

            return (items[index], counts[index]);
        }
    }
}