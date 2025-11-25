using System.Collections.Generic;
using UnityEngine;

public class PlowedLand : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject plantGameObject;
    private IPlant plant;
    private bool wet = false;

    [SerializeField] private SpriteRenderer seedPlaceSpriteRenderer;
    [SerializeField] private SpriteRenderer plantSpriteRenderer;
    [SerializeField] private SpriteRenderer overGroundSpriteRenderer;


    void Awake()
    {
        DayLightHandler._OnTimeReached += ToNextPhasePlant;
    }

    //Метод-событие для смены дня и ночи
    void ToNextPhasePlant((int hh, int mm) time)
    {
        if (plant != null)
        {
            switch (time)
            {
                case (12, 00):
                    if (wet)
                        plant.Grow();   
                    break;
                case (18, 00):
                    if (!wet)
                    {
                        if ((int)plant.plantStatus > 0 && (int)plant.plantStatus < 4)
                        {
                            plant.plantStatus += 4;
                            plant.ToNextPhase();
                       }
                   }
                   break;
            }

            UpdatePlowedLand();
        }
        else
        {
            if (time == (00, 00))
                Destroy(this.gameObject);
        }

    }

    //обновление информации о грядке
    public void UpdatePlowedLand()
    {
        if (plantSpriteRenderer != null)
            plantSpriteRenderer.sprite = plant.PhaseSprite;

        if (plant.plantStatus != 0)
            seedPlaceSpriteRenderer.sprite = null;

        if (wet)
            overGroundSpriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        else
            overGroundSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }


    //реакция объекта на айтем
    (bool, List<IItem>) IInteractable.Interact(IItem item)
    {
        List<IItem> items = new List<IItem>();

        //для мотыги 
        //разрушение культуры: 
        plant = null;
        plantGameObject = null;
        plantSpriteRenderer.sprite = null;
        overGroundSpriteRenderer.sprite = null;

        
        //для семян(универсальный)
        plant = item.GameObject.GetComponent<IPlant>();
        plantGameObject = item.GameObject;
        seedPlaceSpriteRenderer.sprite = plant.PhaseSprite;
        plant.plantStatus = PlantStatus.seed;



        //для руки
        //Зачисление игроку 1 единицы продукта


        //для лейки
        wet = true;


        UpdatePlowedLand();
        return (true, items);
    }

    //поведение при уничтожении объекта
    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= ToNextPhasePlant;
    }
}
