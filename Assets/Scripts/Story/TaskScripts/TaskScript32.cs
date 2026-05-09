using UnityEngine;
using System.Collections;

public class TaskScript32 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField, Range(0, 23)] private int mintargetTime;
    [SerializeField, Range(0, 23)] private int maxtargetTime;
    [SerializeField] private bool beetweenDays;


    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 2 == 0) 
        {
            DayLightHandler._OnTimeReached += CheckTime;
            CheckTime((DayLightHandler.Hours, DayLightHandler.Minutes));
        }
        else
        {
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
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
        if (taskManager.Status % 2 == 0) DayLightHandler._OnTimeReached -= CheckTime;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.Status *= 2;
        DayLightHandler._OnTimeReached += CheckTime;
    }
}
