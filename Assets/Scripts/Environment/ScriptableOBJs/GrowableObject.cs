using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrowableObject", menuName = "GrowableObjects/GrowableObject")]
public class GrowableObject : ScriptableObject
{
    [SerializeField] List<Sprite> GrowingPhaseSprites;
}
