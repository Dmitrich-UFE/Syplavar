using System.Collections;
using UnityEngine;

public class TaskScript11 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;


    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 2 == 0)
        {
            taskManager.TaskHintText = "";
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
        else
        {
            taskManager.TaskHintText = "Может, мотыга поможет?";
            EventManager.OnEventHappened += CheckBrakeStones;
        }
    }

    void CheckBrakeStones(EventMessage msg)
    {
        if (msg.Tag == "BRAKEBUSH" && (int)msg.Data >= 7 && (int)msg.Data <= 9)
        {
            taskManager.Status *= 2;
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.CompleteTask(task.ID);
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckBrakeStones;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }
}
