using System;
using System.Collections.Generic;
using UnityEngine;

public class PlowedLand : MonoBehaviour, IInteractable
{
    private IPlant plant;
    private IGetable getable;
    private bool wet = false;
    private int plantID;

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
            plant = data.Plant;
            if (plant != null)
                plant.plantStatus = (PlantStatus)data.PlantStatus;
        }

        UpdatePlowedLand();
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
            data.Plant = plant;
            data.Type = plant.Type;
        }
        else
        {
            data.Type = PlantTypes.NullPlant;
        }
    
        return data;
    }


    /*public int ID; 7
    public PlantTypes Type; 7
    public Vector3Int Position;7
    public bool Wet; 7
    public PlantStatus PlantStatus; 7
    public IPlant Plant; 7 */ 


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

            PlowedLandManager.Update(GetPlantData());
            UpdatePlowedLand();
        }
        else
        {
            //if (time == (00, 00))
                //Destroy(this.gameObject);
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
        if (plant?.plantStatus != PlantStatus.has_growed )
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
                GameObject _gameObject = Instantiate(item.GameObject, this.transform);
                plant = _gameObject.GetComponent<IPlant>();
                getable =_gameObject.GetComponent<IGetable>();

                plant.plantStatus = PlantStatus.seed;
                plant.GrowingPhase = 0;
                plant.ToNextPhase();
                seedPlaceSpriteRenderer.sprite = plant?.PhaseSprite;

                UpdatePlowedLand();
                PlowedLandManager.Update(GetPlantData());
                return (true, null);
            }

            //для руки
            //Зачисление игроку 1 единицы продукта
            if (item.GameObject.CompareTag("Hand"))
            {

            }

            //для лейки
            if (item.GameObject.CompareTag("WateringCan"))
            {
                IInstrument wateringCan = item.GameObject.GetComponent<IInstrument>();

                (IItem item, bool isSucceed) waterCanReturned = wateringCan.Use();

                if (waterCanReturned.isSucceed)
                {
                    wet = true;
                Debug.Log("растение полито");
                }
            
                PlowedLandManager.Update(GetPlantData());
                UpdatePlowedLand();
                return (false, null);
            }
        }
        else
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
