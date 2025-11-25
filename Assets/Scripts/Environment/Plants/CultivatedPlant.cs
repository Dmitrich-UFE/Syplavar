using System.Collections.Generic;
using UnityEngine;

public class CultivatedPlant : MonoBehaviour, IPlant, IGetable
{
    [SerializeField] private int _growingPhase;
    [SerializeField] private Sprite _phaseSprite;
    [SerializeField] private PlantStatus _plantStatus;
    [SerializeField] private GrowableObject _growingPhasesSprites;
    [SerializeField] private ItemData _returningItem;


    int IPlant.GrowingPhase
    {
        get{ return _growingPhase; } 
        set
        {
            if (value < 0 )
                throw new System.ArgumentOutOfRangeException("value");
            _growingPhase = value;
        } 
    }

    Sprite IPlant.PhaseSprite {
        get 
        {
            return _phaseSprite;
        } 
        set
        {
            _phaseSprite = value;
        } 
    }
    GrowableObject IPlant.growableObject { get; }

    PlantStatus IPlant.plantStatus 
    {
        get { return _plantStatus; }
        set { _plantStatus = value; }
    }

    void IPlant.ToNextPhase()
    {
         _phaseSprite = _growingPhasesSprites.GetGrowingPhase((int)_plantStatus);
    }

    void IPlant.Grow()
    {
        if ((int)_plantStatus < 4)
        {
            _plantStatus++;
            _phaseSprite = _growingPhasesSprites.GetGrowingPhase((int)_plantStatus);
        }
            


    }

    List<IItem> IGetable.Get()
    {
        int count = UnityEngine.Random.Range(1, 2);

        List<IItem> items = new List<IItem>(count);

        for (int i = 0; i < count; i++)
            items.Add(_returningItem);

        return items;
    }
}
