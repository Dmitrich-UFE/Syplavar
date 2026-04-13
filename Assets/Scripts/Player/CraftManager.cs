using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    // ===== DATA =====
    [SerializeField] private RecipeData[] recipes;
    private PlayerInputActions ButtonE;
    [SerializeField] private InventoryAI inventory;

    // ===== UI =====
    [Header("UI")]
    [SerializeField] private GameObject craftWindow;
    [SerializeField] private Transform availableContainer;

    [SerializeField] private Button craftButton;
    [SerializeField] private GameObject recipeButtonPrefab;

    private RecipeData currentRecipe;

    // ===== DRAW MENU =====
    internal void DrawCraftMenu()
    {

        ClearUI();
        Debug.Log("asdsad");

        foreach (var recipe in recipes)
        {
            bool canCraft = CanCraft(recipe);

        

            var buttonUI = Instantiate(recipeButtonPrefab, availableContainer);

            var component = buttonUI.GetComponent<RecipeButtonUI>();

            component.Init(recipe, canCraft, OnRecipeSelected);

            Debug.Log("FDF");
        }
    }

    void Awake()
    {
        ButtonE = new PlayerInputActions();
        ButtonE.Player.OpenBigInventory.performed+=context=>DrawCraftMenu();
    }

    private void ClearUI()
    {
        foreach (Transform child in availableContainer)
            Destroy(child.gameObject);

     
    }

    // ===== CHECK =====

    private bool CanCraft(RecipeData recipe)
    {
        for (int i = 0; i < recipe.IngredientsCount; i++)
        {
            var (item, count) = recipe[i];

            if (inventory.CheckCountOfItem(item) < count)
                return false;
        }

        return true;
    }

    // ===== SELECT =====
    private void OnRecipeSelected(RecipeData recipe)
    {
        currentRecipe = recipe;
        OpenCraftWindow(recipe);
    }

    // ===== OPEN =====
    internal void OpenCraftWindow(RecipeData recipe)
    {
        craftWindow.SetActive(true);
        currentRecipe = recipe;
        UpdateCraftWindow();
    }

    // ===== UPDATE =====
    internal void UpdateCraftWindow()
    {
        if (currentRecipe == null)
            return;

        craftButton.interactable = CanCraft(currentRecipe);
    }

    // ===== CRAFT =====
    internal void Craft()
    {
        if (currentRecipe == null)
            return;

        // ❗ Без доступа к Inventory — просто лог
        Debug.Log("Скрафтили: " + currentRecipe.ResultItem.name);

        UpdateCraftWindow();
        DrawCraftMenu();
    }

    // ===== CLOSE =====
    internal void CloseCraftWindow()
    {
        craftWindow.SetActive(false);

        if (inventory != null)
        {
            inventory.DrawInventory();
        }
    }

    private void OnEnable()
    {
        ButtonE.Enable();
    }

    private void OnDisable()
    {
        ButtonE.Disable();
    }
}