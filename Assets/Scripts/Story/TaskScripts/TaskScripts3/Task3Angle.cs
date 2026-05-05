using UnityEngine;

public class Task3Angle : MonoBehaviour
{
    [SerializeField] private Task3Main task3main;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private InventoryAI inventory;
    [SerializeField] private TaskManager taskManager;
    bool isVisited = false;
    bool isGotItems = false;

    

    void OnTriggerEnter(Collider other)
    {
        if (!isVisited && other.gameObject.CompareTag("Player"))
        {
            vnl.StartPrint(text);
            task3main.CountOfExploredPlaces++;
            isVisited = true;
            getItem();
            //EventManager.OnEventHappened += getItemEvent;
        }
    }

    void getItem()
    {
        if (!isGotItems)
        {
            inventory.AddToInventory(55, 6);
            inventory.AddToInventory(2, 1);
            inventory.AddToInventory(3, 1);
            inventory.AddToInventory(53, 3);
            task3main.CountOfExploredPlaces++;
            isGotItems = true;
            taskManager.Status += 100;
            this.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (taskManager.Status >= 100)
        {
            task3main.CountOfExploredPlaces+=2;
            isGotItems = true;
            this.gameObject.SetActive(false);
        }
    }
}
