using UnityEngine;

public class MiniGrinder : MonoBehaviour, IInstrument
{
    float IInstrument.Damage {get;} = 0.1f;


    (IItem item, bool isSucceed) IInstrument.Use()
    {
        return (null, true);
    }

    void IInstrument.GetRes()
    {
       
    }
}
