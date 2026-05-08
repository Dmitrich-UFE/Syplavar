using UnityEngine;

public class TaskScript17 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject DayLightH;
    [SerializeField] private Bed bed;

    void OnEnable()
    {
        DayLightH.SetActive(true);
        DayLightHandler.SetDayProgress(0.01f);
        bed.UnlockSleep();
        taskManager.TaskHintText = "Что-то спать хочется...";
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

