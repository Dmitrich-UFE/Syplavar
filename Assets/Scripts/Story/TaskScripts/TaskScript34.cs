using UnityEngine;

public class TaskScript34 : MonoBehaviour
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
        if (msg.Tag == "GETITEM" && inventory.CheckCountOfItemByID(601) > 0 &&
        inventory.CheckCountOfItemByID(602) > 0 && inventory.CheckCountOfItemByID(603) > 0)
        {
            taskManager.CompleteTask(task.ID);
        }
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckItems;
    }
}
