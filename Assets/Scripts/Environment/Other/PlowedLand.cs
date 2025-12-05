using System.Collections.Generic;
using UnityEngine;

public class PlowedLand : MonoBehaviour, IInteractable
{
    private IPlant plant;
    private IGetable getable;
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
                            Debug.Log("растение высохло");
                            plant.plantStatus += 4;
                            plant.ToNextPhase();
                        }
                    }
                    else
                        wet = false;
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
    private void UpdatePlowedLand()
    {
        if (plantSpriteRenderer != null)
            plantSpriteRenderer.sprite = plant?.PhaseSprite;
        else
        {
            seedPlaceSpriteRenderer.sprite = null;
            plantSpriteRenderer.sprite = null;
        }

        if (plant?.plantStatus != 0)
            seedPlaceSpriteRenderer.sprite = null;
        else
            plantSpriteRenderer.sprite = null;

        if (wet)
            overGroundSpriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        else
            overGroundSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }


    //реакция объекта на айтем
    (bool, List<IItem>) IInteractable.Interact(IItem item)
    {
        //для мотыги 
        //разрушение культуры: 
        if(item.GameObject.CompareTag("Hoe"))
        {
            ClearCulture();

            UpdatePlowedLand();
            return (false, null);
        }  
        
        //для семян(универсальный)
        if(item.GameObject.CompareTag("Plant"))
        {
            GameObject _gameObject = Instantiate(item.GameObject, this.transform);
            plant = _gameObject.GetComponent<IPlant>();
            getable =_gameObject.GetComponent<IGetable>();

            plant.plantStatus = PlantStatus.seed;
            plant.GrowingPhase = 0;
            plant.ToNextPhase();
            seedPlaceSpriteRenderer.sprite = plant?.PhaseSprite;

            UpdatePlowedLand();
            return (true, null);
        }

        //для руки
        //Зачисление игроку 1 единицы продукта
        if (item.GameObject.CompareTag("Hand"))
        {
            Debug.Log("растение полито");
            wet = true;
            UpdatePlowedLand();

            if (plant?.plantStatus == PlantStatus.has_growed)
            {
                List<IItem> items = new List<IItem>(getable.Get());
                ClearCulture();

                UpdatePlowedLand();
                return (false, items);
            }    
        }

        //для лейки
        if (item.GameObject.CompareTag("WateringCan"))
        {
            Debug.Log("растение полито");
            wet = true;

            UpdatePlowedLand();
            return (false, null);
        }


        UpdatePlowedLand();
        return (false, null);
    }

    //поведение при уничтожении объекта
    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= ToNextPhasePlant;
    }

    //разрушение культуры: 
    void ClearCulture()
    {
        plant = null;
        getable = null;
        plantSpriteRenderer.sprite = null;
        seedPlaceSpriteRenderer.sprite = null;
    }
}
