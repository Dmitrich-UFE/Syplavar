using UnityEngine;
using System.Collections.Generic;

public class miniFlower : MonoBehaviour, IInteractable
{
    [SerializeField] private int BrokenByItemID;
    [SerializeField] private int queue;

    (bool isDebitNeed, List<IItem> gettingItems) IInteractable.Interact(IItem item)
    {
        if (item == null)
        {
            return (false, null);
        }

        ItemData itemData = item as ItemData;

        if (itemData != null && itemData.ItemID == BrokenByItemID)
        {
            gameObject.SetActive(false);
            EventManager.SendEvent("BREAKMINIFLOWER", queue);
        }

        return (false, null);
    }
}
