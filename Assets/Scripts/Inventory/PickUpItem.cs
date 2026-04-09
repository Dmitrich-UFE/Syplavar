using UnityEngine;
using System.Collections.Generic;

public class PickUpItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData returningItem;
    [SerializeField] private SpriteRenderer spriteVertical;
    [SerializeField] private SpriteRenderer spriteOnGround;

    [SerializeField] private bool onGround;
    [SerializeField] private int countOfItems;
    [SerializeField] private bool multiusing;


    void Awake()
    {
        if (onGround)
        {
            spriteVertical.sprite = null;
            spriteOnGround.sprite = returningItem.Texture;
        }
        else
        {
            spriteOnGround.sprite = null;
            spriteVertical.sprite = returningItem.Texture;
        }
    }


    (bool isDebitNeed, List<IItem> gettingItems) IInteractable.Interact(IItem item)
    {
        if (returningItem == null) return  (false, null);
        List<IItem> retItems= new List<IItem>();
        for (int i = 0; i < countOfItems; i++)
        {
            if (returningItem != null)
            {
                retItems.Add(returningItem);
            }
        }

        if (!multiusing)
        {
            returningItem = null;
        }

        return (false, retItems);
    }

    void Update()
    {
        if (returningItem == null)
        {
            Destroy(this.gameObject);
        }
    }

}
