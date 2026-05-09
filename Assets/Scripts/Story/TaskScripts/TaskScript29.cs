using UnityEngine;
using System.Collections;

public class TaskScript29 : MonoBehaviour
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
            EventManager.OnEventHappened += CheckItems;
            CheckItems(new EventMessage("GETITEM", 1));
        }
        else
        {
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
    }

    void CheckItems(EventMessage msg)
    {
        if (msg.Tag == "GETITEM" && taskManager.Status % 2 == 0 && inventory.CheckCountOfItemByID(101) >= 6)
        {
            EventManager.OnEventHappened -= CheckItems;
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
        EventManager.OnEventHappened += CheckItems;
    }

    void OnDisable()
    {
        EventManager.OnEventHappened -= CheckItems;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }
}
