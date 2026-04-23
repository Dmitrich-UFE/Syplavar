using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("CraftUIWindow")]
    [SerializeField] private Image[] slotImages;

    [SerializeField] private TMP_Text[] slotCounts;
    [SerializeField] private Image slotResultImage;
    [SerializeField] private TMP_Text slotResultCount;

    private RecipeData currentRecipe;

    // ===== DRAW MENU =====
    internal void DrawCraftMenu()
    {

        ClearUI();

        foreach (var recipe in recipes)
        {
            bool canCraft = CanCraft(recipe);

            var buttonUI = Instantiate(recipeButtonPrefab, availableContainer);

            var component = buttonUI.GetComponent<RecipeButtonUI>();

            component.Init(recipe, canCraft, OnRecipeSelected);
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
        for (int i = 0; i<recipe.IngredientsCount;++i)
        {
            var (item, count) = recipe[i];
            slotImages[i].sprite = item.Texture; 
            slotCounts[i].text = count.ToString();
        }

        slotResultImage.sprite = recipe.ResultItem.Texture;
        slotResultCount.text= recipe.ResultCount.ToString();

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
    public void Craft()
    {
        if (currentRecipe == null)
            return;

        bool canCraft = true;

        for (int i = 0; i < currentRecipe.IngredientsCount; i++)
        {   
            var recipeItem = currentRecipe[i];
            canCraft = canCraft && (inventory.CheckCountOfItem(recipeItem.item) > recipeItem.count);
        }

        if (canCraft)
        {
            for (int i = 0; i < currentRecipe.IngredientsCount; i++)
            {   
                var recipeItem = currentRecipe[i];
                inventory.DebitItem(recipeItem.item, recipeItem.count);
            }
            Debug.Log("Скрафтили: " + currentRecipe.ResultItem.name);
            inventory.AddToInventory(currentRecipe.ResultItem, currentRecipe.ResultCount);
        }
        

        UpdateCraftWindow();
        DrawCraftMenu();
        inventory.DrawInventory();
    }

    // ===== CLOSE =====
    public void CloseCraftWindow()
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