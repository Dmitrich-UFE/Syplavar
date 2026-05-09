using System;
using System.Collections.Generic;
using UnityEngine;

public class PlowedLand : MonoBehaviour, IInteractable
{
    private IPlant plant;
    private IGetable getable;
    private bool wet = false;
    private int plantID;
    private GameObject _plantObj;

    [SerializeField] private SpriteRenderer seedPlaceSpriteRenderer;
    [SerializeField] private SpriteRenderer plantSpriteRenderer;
    [SerializeField] private SpriteRenderer overGroundSpriteRenderer;
    [SerializeField] private bool IsGenerateByManager;


    void Awake()
    {
        DayLightHandler._OnTimeReached += ToNextPhasePlant;
        if (!IsGenerateByManager)
        {
            GeneratePlowedLandDataWhenPlayerCreates();
        }
    }

    internal void GeneratePlowedLandDataWhenPlayerCreates()
    {
        plantID = PlowedLandManager.GetID();
        PlowedLandData data = GetPlantData();
        data.ID = plantID;
        PlowedLandManager.Update(data);
        UpdatePlowedLand();
    }

    internal void Init(PlowedLandData data)
    {
        plantID = data.ID;
        wet = data.Wet;
        this.transform.position = data.Position;

        if (data.Plant != null)
        {
            _plantObj = data.Plant;
            plant = data.Plant.GetComponent<IPlant>();
            getable = data.Plant.GetComponent<IGetable>();
            if (plant != null)
            {
                plant.plantStatus = data.PlantStatus;
                plant.ToNextPhase();
            }
        }

        UpdatePlowedLand();
        PlowedLandManager.Update(GetPlantData());
    }

    internal PlowedLandData GetPlantData()
    {
        PlowedLandData data = new PlowedLandData();
        data.ID = plantID;
        data.Wet = wet;
        Vector3 actPos = this.transform.position;
        data.Position = new Vector3Int((int)Math.Round(actPos.x), (int)Math.Round(actPos.y), (int)Math.Round(actPos.z));

        if (plant != null)
        {
            data.PlantStatus = plant.plantStatus;
            data.Plant = _plantObj;
            data.Type = plant.Type;
        }
        else
        {
            data.Type = PlantTypes.NullPlant;
        }
    
        return data;
    }

    //Метод-событие для смены дня и ночи
    void ToNextPhasePlant((int hh, int mm) time)
    {
        if (plant != null)
        {
            switch (time.hh)
            {
                case 7:
                    if (wet)
                        plant.Grow();   
                    break;
                case 18:
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
        }
        else
        {
            if (time.hh == 18)
                wet = false;
        }
        
        PlowedLandManager.Update(GetPlantData());
        UpdatePlowedLand();
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
        {
            plantSpriteRenderer.sprite = plant?.PhaseSprite;
            seedPlaceSpriteRenderer.sprite = null;
        }
        else
        {
            seedPlaceSpriteRenderer.sprite = plant?.PhaseSprite;
            plantSpriteRenderer.sprite = null;
        }

        if (wet)
            overGroundSpriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        else
            overGroundSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }


    //реакция объекта на айтем
    (bool, List<IItem>) IInteractable.Interact(IItem item)
    {
        if (plant?.plantStatus != PlantStatus.has_growed)
        {
            if (item.GameObject == null)
            {
                return (false, null);
            }

            //для мотыги 
            //разрушение культуры: 
            if(item.GameObject.CompareTag("Hoe"))
            {
                if (plant == null)
                {
                    Destroy(this.gameObject);
                    plantID = -plantID;
                }

                ClearCulture();

                UpdatePlowedLand();
                
                PlowedLandManager.Update(GetPlantData());
                return (false, null);
            }  
        
            //для семян(универсальный)
            if(item.GameObject.CompareTag("Plant"))
            {
                _plantObj = Instantiate(item.GameObject, this.transform);
                plant = _plantObj.GetComponent<IPlant>();
                getable = _plantObj.GetComponent<IGetable>();

                plant.plantStatus = PlantStatus.seed;
                plant.GrowingPhase = 0;
                plant.ToNextPhase();
                seedPlaceSpriteRenderer.sprite = plant?.PhaseSprite;

                UpdatePlowedLand();
                PlowedLandManager.Update(GetPlantData());

                EventManager.SendEvent("PLANTPLANT", 1);
                return (true, null);
            }

            //для лейки
            if (item.GameObject.CompareTag("WateringCan"))
            {
                IInstrument wateringCan = item.GameObject.GetComponent<IInstrument>();

                (IItem item, bool isSucceed) waterCanReturned = wateringCan.Use();

                if (waterCanReturned.isSucceed)
                {
                    wet = true;
                    EventManager.SendEvent("WATERPLANT", 1);
                }
            
                PlowedLandManager.Update(GetPlantData());
                UpdatePlowedLand();
                return (false, null);
            }
        }
        else if (getable != null)
        {
            List<IItem> items = new List<IItem>(getable.Get());
            ClearCulture();

            UpdatePlowedLand();

            PlowedLandManager.Update(GetPlantData());
            return (false, items);
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
