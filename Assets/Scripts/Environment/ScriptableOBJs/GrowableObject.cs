using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrowableObject", menuName = "GrowableObjects/GrowableObject")]
public class GrowableObject : ScriptableObject
{
    [SerializeField] List<Sprite> GrowingPhaseSprites;

    public Sprite GetGrowingPhase(int index)
    {
        return GrowingPhaseSprites[index];
    }
}
