using UnityEngine;

public class PlowedLand : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject plantGameObject;
    private IPlant plant;
    private bool wet = false;
    [SerializeField] private SpriteRenderer plantSpriteRenderer;
    [SerializeField] private SpriteRenderer overGroundSpriteRenderer;


    void Awake()
    {
        DayLightHandler._OnTimeReached += ToNextPhasePlant;
    }

    void ToNextPhasePlant((int hh, int mm) time)
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

    }


    //Метод-событие для смены дня и ночи

    void IInteractable.Interact(IItem item)
    {
        //для мотыги 
        //разрушение культуры: 
        plant = null;
        plantGameObject = null;
        plantSpriteRenderer.sprite = null;
        overGroundSpriteRenderer.sprite = null;

        
        //для семян(универсальный)
        plant = item.GameObject.GetComponent<IPlant>();
        plantGameObject = item.GameObject;
        overGroundSpriteRenderer.sprite = plant.PhaseSprite;
        plant.plantStatus = PlantStatus.seed;



        //для руки
        //Зачисление игроку 1 единицы продукта

        //для лейки
        wet = true;
    }
}
