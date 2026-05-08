using UnityEngine;

public class TaskScript14 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject DayLightH;
    [SerializeField] private Bed bed;

    void OnEnable()
    {
        EventManager.OnEventHappened += CheckSleep;
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckSleep;
        DayLightHandler._OnTimeReached -= CheckTime;
    }

    void CheckSleep(EventMessage msg)
    {
        if (msg.Tag == "SLEEPSTARTED")
        {
            taskManager.OpenBlackPanel();
            DayLightHandler._OnTimeReached += CheckTime;
            //taskManager.CompleteTask(task.ID);
        }
    }

    void CheckTime((int hh, int mm) time)
    {
        if (time.hh >= 0)
        {
            taskManager.CloseBlackPanelNoFade();
            bed.BlockSleep();
            DayLightHandler.SetDayProgress(0.01f);
            DayLightH.SetActive(false);
            taskManager.CompleteTask(task.ID);
        }
    }
}
