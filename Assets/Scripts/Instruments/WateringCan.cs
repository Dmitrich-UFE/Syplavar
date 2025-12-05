using UnityEngine;

public class WateringCan: MonoBehaviour, IInstrument
{
    [SerializeField] private uint _maxCapacity;
    float IInstrument.Damage {get;} = 0;
    uint _waterCapaсity = 0;

   (IItem item, bool isSucceed) IInstrument.Use()
    {
        if (_waterCapaсity > 0)
        {
            --_waterCapaсity;
            Debug.Log("лейка использована");
            return (null, true);
        }

        return (null, false);
    }

    void IInstrument.GetRes()
    {
        if (_waterCapaсity <_maxCapacity)
        {
            Debug.Log("Лейка наполнена");
            _waterCapaсity = _maxCapacity;
        }
    }



}
