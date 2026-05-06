using UnityEngine;
using System.Collections;
using System.Net.Http.Headers;

public class TaskScript6 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset textAfterWatering;
    [SerializeField] private TextAsset textBeforeWatering;
    [SerializeField] private BoxCollider boxCollider;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        
        DayLightHandler._OnTimeReached += CheckTime;
        
        if (taskManager.Status % 59049 == 0)
        {
            vnl.StartPrint(textAfterWatering);
            cor = StartCoroutine(checkStatus2());
        }
        if (taskManager.Status % 7 == 0)
        {
            boxCollider.enabled = false;
            taskManager.TaskHintText = "Нужно набрать лейку - наведитесь на колодец и нажмите ПКМ";
            EventManager.OnEventHappened += CheckWateringCan;
        }
    }

    void CheckTime((int hh, int mm) time)
    {
        if (time.hh >= 18 || time.hh < 7)
        {
            taskManager.Status *=2;
        }
        else if (taskManager.Status % 2 == 0)
        {
            taskManager.Status /=2;
        }
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckWateringCan;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    void CheckWateringCan(EventMessage msg)
    {
        if (msg.Tag == "FULLCAN")
        {
            taskManager.Status *= 5;
            taskManager.TaskHintText = "";
            EventManager.OnEventHappened -= CheckWateringCan;
            EventManager.OnEventHappened += CheckPlantWatering;
        }
    }

    void CheckPlantWatering(EventMessage msg)
    {
        if (msg.Tag == "WATERPLANT" && taskManager.Status % 2 == 0)
        {
            taskManager.Status *= 3;
        }
        if (taskManager.Status % 19683 == 0)
        {
            EventManager.OnEventHappened -= CheckPlantWatering;
            vnl.StartPrint(textAfterWatering);
            cor = StartCoroutine(checkStatus2());
        }
    }

    

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.TaskHintText = "Нужно набрать лейку - наведитесь на колодец и нажмите ПКМ";
        EventManager.OnEventHappened += CheckWateringCan;
        taskManager.Status *=7;
        //taskManager.CompleteTask(task.ID);
    }

    IEnumerator checkStatus2()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }
        taskManager.CompleteTask(task.ID);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            vnl.StartPrint(textBeforeWatering);
            cor = StartCoroutine(checkStatus());
            boxCollider.enabled = false;
        }
    }
}
