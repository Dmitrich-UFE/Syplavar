using UnityEngine;
using System.Collections;

public class TaskScript36 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject miniFlower;
    [SerializeField] private int targetType;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        miniFlower.SetActive(true);
        if (taskManager.Status % 2 == 0) 
        {
            EventManager.OnEventHappened += CheckEvent;
        }
        else
        {
            if (vnl != null && text != null) 
            {
                vnl.StartPrint(text);
                cor = StartCoroutine(checkStatus());
            }
            else
            {
                taskManager.Status *= 2;
                EventManager.OnEventHappened += CheckEvent;
            }
        }
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckEvent;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    void CheckEvent(EventMessage msg)
    {
        if (msg.Tag == "BREAKMINIFLOWER" && (int)msg.Data == targetType)
        {
            taskManager.CompleteTask(task.ID);
        }
    }
    
    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.Status *= 2;
        EventManager.OnEventHappened += CheckEvent;
    }


}
