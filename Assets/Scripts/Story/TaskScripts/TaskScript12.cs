using System.Collections;
using UnityEngine;

public class TaskScript12 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private InventoryAI inventory;

    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 2 == 0)
        {
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
        else
        {
            EventManager.OnEventHappened += CheckItems;
        }
    }

    void CheckItems(EventMessage msg)
    {
        if (msg.Tag == "GETITEM" && inventory.CheckCountOfItemByID(100) > 5)
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
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }
}
