using UnityEngine;

public class Cabbage : MonoBehaviour, IPlant
{
    [SerializeField] private int _growingPhase;
    [SerializeField] private Sprite _phaseSprite;
    [SerializeField] private PlantStatus _plantStatus;
    [SerializeField] private GrowableObject _growingPhasesSprites;



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


    }

    void IPlant.Grow()
    {
        if ((int)_plantStatus < 4)
        {
            _plantStatus++;
            _phaseSprite = _growingPhasesSprites.GetGrowingPhase((int)_plantStatus);
        }
            


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
