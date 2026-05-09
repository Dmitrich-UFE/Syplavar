using UnityEngine;

public class TaskScript19 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField, Range(0, 23)] private int mintargetTime;
    [SerializeField, Range(0, 23)] private int maxtargetTime;
    [SerializeField] private bool beetweenDays;

    void OnEnable()
    {
        DayLightHandler._OnTimeReached += CheckTime;
        CheckTime((DayLightHandler.Hours, DayLightHandler.Minutes));
    }

    void CheckTime((int hh, int mm) time)
    {
        if (beetweenDays && (time.hh <= mintargetTime || time.hh >= maxtargetTime))
        {
            taskManager.CompleteTask(task.ID);
        }

        if (!beetweenDays && time.hh >= mintargetTime && time.hh <= maxtargetTime)
        {
            taskManager.CompleteTask(task.ID);
        }
    }
   
    void OnDisable()
    {
        DayLightHandler._OnTimeReached -= CheckTime;
    }
}
