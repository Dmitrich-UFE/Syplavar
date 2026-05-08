using UnityEngine;

public class TaskScript7 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private string hintText = "Наведитесь на кровать и нажмите ПКМ";

    void OnEnable()
    {
        taskManager.TaskHintText = hintText;
        EventManager.OnEventHappened += CheckSleep;
    }

    void OnDisable()
    {
        taskManager.TaskHintText = "";
        EventManager.OnEventHappened -= CheckSleep;
    }

    void CheckSleep(EventMessage msg)
    {
        if (msg.Tag == "SLEEPCOMPLETED")
        {
            taskManager.CompleteTask(task.ID);
        }
    }
}
