using UnityEngine;

public interface IInstrument
{
    float Damage { get; }

    (IItem item, bool isSucceed) Use();

    void GetRes();
}
