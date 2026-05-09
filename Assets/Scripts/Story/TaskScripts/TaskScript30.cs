using UnityEngine;

public class TaskScript30 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private InventoryAI inventory;

    void OnEnable()
    {
        EventManager.OnEventHappened += CheckItems;
        CheckItems(new EventMessage("GETITEM", 1));
    }

    void CheckItems(EventMessage msg)
    {
        if (msg.Tag == "GETITEM" && (inventory.CheckCountOfItemByID(1007) > 0 ||
        inventory.CheckCountOfItemByID(1008) > 0 || inventory.CheckCountOfItemByID(1006) > 0 ||
        inventory.CheckCountOfItemByID(1004) > 0 ))
        {
            taskManager.CompleteTask(task.ID);
        }
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckItems;
    }
}
