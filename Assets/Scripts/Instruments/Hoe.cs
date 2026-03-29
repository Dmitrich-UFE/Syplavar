using UnityEngine;

public class Hoe : MonoBehaviour, IInstrument
{
    float IInstrument.Damage {get;} = 3;


    (IItem item, bool isSucceed) IInstrument.Use()
    {
        return (null, true);
    }

    void IInstrument.GetRes()
    {
       
    }
}
