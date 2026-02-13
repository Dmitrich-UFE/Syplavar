using UnityEngine;
using System.Collections.Generic;

public class Tree : MonoBehaviour, IInteractable
{
    
    [SerializeField] private float health;
    [SerializeField] private bool isGrowed;
    private int daysBeforeGrow;
    [SerializeField] private ItemData[] returningItems;

    [SerializeField] private GameObject actualTreeObject;
    [SerializeField] private List<GameObject> GrowedTreeObjects;


    void Awake()
    {
        daysBeforeGrow = Random.Range(2, 10);
        DayLightHandler._OnTimeReached += Growing;
        Instantiate(actualTreeObject, this.transform);
    }


    (bool isDebitNeed, List<IItem> gettingItems) IInteractable.Interact(IItem item)
    {
        if (item.GameObject == null)
        {
            return (false, null);
        }


        if(item.GameObject.CompareTag("Axe"))
        {
            IInstrument axe = item.GameObject.GetComponent<IInstrument>();
            health -= axe?.Damage ?? 0;

            if (health <= 0)
            {
                if (isGrowed)
                {
                    //destroy the object
                    List<IItem> retItems = new List<IItem>();

                    foreach (ItemData returningitem in returningItems)
                    {
                        for (int i = 0; i < Random.Range(3, 6); i++)
                        {
                            retItems.Add(returningitem);
                        }
                    }

                    Destroy(actualTreeObject);
                    actualTreeObject = null;
                    return (false, retItems);
                }

                else
                {
                    Destroy(actualTreeObject);
                    actualTreeObject = null;
                    return (false, null);
                }
            }

            
        }  

        return (false, null);
    }


    void Growing((int hh, int mm) time)
    {
        switch (time)
        {
            case (6, 00):
                daysBeforeGrow -= 1;

                if (daysBeforeGrow == 0)
                {
                    daysBeforeGrow = -99;
                    Growed();
                }
                break;
            case (0, 0):
                if (actualTreeObject == null)
                    Destroy(this);
                break;
        }
    }

    void Growed()
    {
        actualTreeObject = Instantiate(GrowedTreeObjects[Random.Range(0, GrowedTreeObjects.Count - 1)], this.transform);
        health = Random.Range(10, 70);
    }

    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= Growing;
    }



}
