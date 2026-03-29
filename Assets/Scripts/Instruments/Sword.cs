using UnityEngine;

public class Sword : MonoBehaviour, IInstrument
{
    float IInstrument.Damage {get;} = 7;


    (IItem item, bool isSucceed) IInstrument.Use()
    {
        return (null, true);
    }

    void IInstrument.GetRes()
    {
       
    }
}
