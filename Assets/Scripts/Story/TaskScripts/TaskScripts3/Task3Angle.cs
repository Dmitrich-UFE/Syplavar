using UnityEngine;
using System.Collections;

public class Task3Angle : MonoBehaviour
{
    [SerializeField] private Task3Main task3main;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private InventoryAI inventory;
    [SerializeField] private TaskManager taskManager;
    bool isVisited = false;
    bool isGotItems = false;

    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    

    void OnTriggerEnter(Collider other)
    {
        if (!isVisited && other.gameObject.CompareTag("Player"))
        {
            vnl.StartPrint(text);
            task3main.CountOfExploredPlaces++;
            isVisited = true;
            cor = StartCoroutine(checkStatus());
            //EventManager.OnEventHappened += getItemEvent;
        }
    }

    void getItem()
    {
        if (!isGotItems)
        {
            inventory.AddToInventory(55, 10);
            inventory.AddToInventory(2, 1);
            inventory.AddToInventory(3, 1);
            inventory.AddToInventory(53, 4);
            task3main.CountOfExploredPlaces++;
            isGotItems = true;
            taskManager.Status *= 5;
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }
        getItem();
    }

    void OnEnable()
    {
        if (taskManager.Status % 5 == 0)
        {
            task3main.CountOfExploredPlaces+=2;
            isGotItems = true;
            this.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }
}
