using UnityEngine;
using System.Collections;

public class TaskScript4 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 512 != 0)
        {
            EventManager.OnEventHappened += CheckPlants;
            taskManager.TaskHintText = "Возможно, мотыга поможет. Чтобы использовать инструмент, нажмите ПКМ";
        } 
        else
        {
            StartDialogue();
        }
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckPlants;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    void CheckPlants(EventMessage msg)
    {
        if (msg.Tag == "PLANTPLANT") taskManager.Status *= 2;

        if (taskManager.Status % 512 == 0)
        {
            EventManager.OnEventHappened -= CheckPlants;
            taskManager.TaskHintText = "";
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        vnl.StartPrint(text);
        cor = StartCoroutine(checkStatus());
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.CompleteTask(task.ID);
    }

}
