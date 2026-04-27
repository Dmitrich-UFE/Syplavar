using UnityEngine;
using System.Collections.Generic;
using System;

public class Bush : MonoBehaviour, IInteractable
{
    [Header("BASEN")]
    [SerializeField] private int type;
    private int id;

    [Header("Returning items (50% for each)")]
    [SerializeField] private ItemData[] returningItems50;

    [Header("Returning items (10% for each)")]
    [SerializeField] private ItemData[] returningItems10;

    void Awake()
    {
        
    }

    internal void Init(BushData data)
    {
        if (data != null)
        {
            id = data.ID;
            type = data.Type;
        }
    }


    internal BushSaveData GetSaveData()
    {
        BushSaveData saveData = new BushSaveData{Type = type};
        Vector3Int coord = new Vector3Int((int)Math.Round(transform.position.x), 0, (int)Math.Round(transform.position.z));
        saveData.Position = coord;
        return saveData;
    }

    (bool isDebitNeed, List<IItem> gettingItems) IInteractable.Interact(IItem item)
    {
        if (item.GameObject == null)
        {
            return (false, null);
        }

        IInstrument instrument = item.GameObject.GetComponent<IInstrument>();

        if (instrument != null)
        {
            Destroy(this.gameObject);

            List<IItem> retItems = new List<IItem>();

            foreach (ItemData returningitem in returningItems50)
            {
                int chance = UnityEngine.Random.Range(0, 10);
                if (chance > 5)
                {
                    retItems.Add(returningitem);
                }
            }

            foreach (ItemData returningitem in returningItems10)
            {
                int chance = UnityEngine.Random.Range(0, 10);
                if (chance > 8)
                {
                    retItems.Add(returningitem);
                }
            }

            return (false, retItems);
        }

        return (false, null);
    }


}
