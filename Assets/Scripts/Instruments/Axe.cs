using UnityEngine;

public class Axe : MonoBehaviour, IInstrument
{
    float IInstrument.Damage {get;} = 5;


    (IItem item, bool isSucceed) IInstrument.Use()
    {
        return (null, true);
    }

    void IInstrument.GetRes()
    {
       
    }
}
