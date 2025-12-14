using System.Collections.Generic;
using UnityEngine;

public class UnplowedLand : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _plowedLandPrefab;
    [SerializeField] private Cursor _cursor;

    private void Awake()
    {
        
    }

    public (bool isDebitNeed, List<IItem> gettingItems) Interact(IItem item)
    {
        if (item.GameObject != null && item.GameObject.CompareTag("Hoe") && !(_cursor.interactableObject is PlowedLand))
        {
            Instantiate(_plowedLandPrefab, _cursor.gameObject.transform.position, Quaternion.identity);

            return (false, null);
        }

        return (false, null);
    }
}