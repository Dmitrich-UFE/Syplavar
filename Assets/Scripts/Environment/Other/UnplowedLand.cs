using System.Collections.Generic;
using UnityEngine;

public class UnplowedLand : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _plowedLandPrefab;
    [SerializeField] private SpriteRenderer _groundSpriteRenderer;
    private void Awake()
    {
        
    }

    public (bool isDebitNeed, List<IItem> gettingItems) Interact(IItem item)
    {
        if (item.GameObject.CompareTag("Instrument"))
        {
            GameObject plowedLand = Instantiate(_plowedLandPrefab, transform.position, transform.rotation, transform.parent);

            Destroy(this.gameObject);
            return (false, null);
        }

        return (false, null);
    }
}