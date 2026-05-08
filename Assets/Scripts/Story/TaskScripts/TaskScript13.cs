using UnityEngine;

public class TaskScript13 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    void OnEnable()
    {
        if (taskManager.Status % 512 == 0)
        {
            taskManager.CompleteTask(task.ID);
        }
        else
        {
            DayLightHandler._OnTimeReached += CheckTime;
            EventManager.OnEventHappened += CheckPlantWatering;
        }
    }

    void OnDisable()
    {
        DayLightHandler._OnTimeReached -= CheckTime;
        EventManager.OnEventHappened -= CheckPlantWatering;
    }

    void CheckTime((int hh, int mm) time)
    {
        if (time.hh >= 18 || time.hh < 7)
        {
            taskManager.Status *=3;
        }
        else if (taskManager.Status % 3 == 0)
        {
            taskManager.Status /=3;
        }
    }

    void CheckPlantWatering(EventMessage msg)
    {
        if (msg.Tag == "WATERPLANT" && taskManager.Status % 3 == 0)
        {
            taskManager.Status *= 2;
        }
        if (taskManager.Status % 512 == 0)
        {
             taskManager.CompleteTask(task.ID);
        }
    }
}

