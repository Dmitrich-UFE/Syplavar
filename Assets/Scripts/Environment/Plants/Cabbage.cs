using UnityEngine;

public class Cabbage : MonoBehaviour, IPlant, IInteractable
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
    PlantStatus IPlant.plantStatus { get; set; }

    void IPlant.ToNextPhase()
    {


    }

    void IPlant.Grow()
    {


    }


    void IInteractable.Interact(IItem item){}




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
