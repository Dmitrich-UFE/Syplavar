using System.Collections.Generic;
using UnityEngine;

public class UnplowedLand : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _plowedLandPrefab;
    [SerializeField] private Transform CursorTransform;

    private void Awake()
    {
        
    }

    public (bool isDebitNeed, List<IItem> gettingItems) Interact(IItem item)
    {
        if (item.GameObject.CompareTag("Hoe"))
        {
            Instantiate(_plowedLandPrefab, CursorTransform.position, Quaternion.identity);

            return (false, null);
        }

        return (false, null);
    }
}