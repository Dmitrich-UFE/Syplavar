using UnityEngine;

public class TaskScript31 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    void OnEnable()
    {
        taskManager.TaskHintText = "О, сейчас поем... (Зажмите ПКМ на еде)";
        EventManager.OnEventHappened += CheckItems;
        CheckItems(new EventMessage("GETITEM", 1));
    }

    void CheckItems(EventMessage msg)
    {
        if (msg.Tag == "EATFOOD" && ((int)msg.Data == 1004 || (int)msg.Data >= 1006 && (int)msg.Data <= 1008))
        {
            taskManager.CompleteTask(task.ID);
        }
    }

    void OnDisable()
    {
        taskManager.TaskHintText = "";
        EventManager.OnEventHappened -= CheckItems;
    }
}
