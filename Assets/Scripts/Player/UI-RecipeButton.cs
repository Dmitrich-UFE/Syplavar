using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class RecipeButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image[] slotImages;

    [SerializeField] private TMP_Text[] slotCounts;
    [SerializeField] private Image slotResultImage;
    [SerializeField] private TMP_Text slotResultCount;



    private RecipeData recipe;

    public void Init(RecipeData recipe, bool interactable, Action<RecipeData> onClick)
    {
        for (int i = 0; i<recipe.IngredientsCount;++i)
        {
            var (item, count) = recipe[i];
            slotImages[i].sprite = item.Texture; 
            slotCounts[i].text= count.ToString();
        }

        slotResultImage.sprite = recipe.ResultItem.Texture;
        slotResultCount.text= recipe.ResultCount.ToString();
        this.recipe = recipe;

        button.interactable = interactable;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(recipe));
    }


}