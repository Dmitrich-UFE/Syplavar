using UnityEngine;
using System.Collections.Generic;

public class Well : MonoBehaviour, IInteractable
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    (bool, List<IItem>) IInteractable.Interact(IItem item)
    {
        GameObject gameObject = item.GameObject;

        if (item.GameObject.CompareTag("WateringCan"))
        {
            IInstrument _wateringCan = gameObject.GetComponent<IInstrument>();

            _wateringCan.GetRes();
        }

        return (false, null);
    }
}
