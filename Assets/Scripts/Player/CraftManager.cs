using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class CraftManager : MonoBehaviour
{
    // ===== DATA =====
    [SerializeField] private List<RecipeData> recipes;
    private PlayerInputActions ButtonE;
    [SerializeField] private InventoryAI inventory;

    // ===== UI =====
    [Header("UI")]
    [SerializeField] private GameObject craftWindow;
    [SerializeField] private Transform availableContainer;

    [SerializeField] private Button craftButton;
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private Sprite defaultSprite;

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

        recipes = recipes.OrderByDescending(x => CanCraft(x)).ToList();

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
        for (int i = 0; i<slotImages.Length; ++i)
        {
            if (i < recipe.IngredientsCount)
            {
                var (item, count) = recipe[i];
                slotImages[i].sprite = item.Texture;
                slotCounts[i].text = count.ToString();
            }
            else
            {
                // Если ингредиентов меньше, заполняем дефолтными значениями
                slotImages[i].sprite = defaultSprite;
                slotCounts[i].text = "";
            }
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

        if (CanCraft(currentRecipe))
        {
            for (int i = 0; i < currentRecipe.IngredientsCount; i++)
            {   
                var recipeItem = currentRecipe[i];
                inventory.DebitItem(recipeItem.item, recipeItem.count);
                if (recipeItem.item.GameObject != null)
                {
                    IInstrument instrument = recipeItem.item.GameObject.GetComponent<IInstrument>();
                    if (instrument != null)
                    {
                        inventory.AddToInventory(recipeItem.item, recipeItem.count);
                    }
                }
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

    internal void AddRecipe(RecipeData recipe)
    {
        if (recipe != null)
            recipes.Add(recipe);
    }

    internal void RemoveRecipe(RecipeData recipe)
    {
        if (recipe != null)
            recipes.Remove(recipe);
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