using UnityEngine;
using System.Collections;

public class TaskScript33 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private InventoryAI inventory;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private CentralSaveSystem saveSystem;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 2 != 0) 
        {
            taskManager.TaskHintText = "Есть ли способы сделать иранам?";
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
        if (msg.Tag == "GETITEM" && inventory.CheckCountOfItemByID(103) >=6 &&
        inventory.CheckCountOfItemByID(104) >= 7)
        {
            taskManager.Status *= 2;
            saveSystem.SaveData();
            EventManager.OnEventHappened -= CheckItems;
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
    }

    void OnDisable()
    {
        taskManager.TaskHintText = "";
        EventManager.OnEventHappened -= CheckItems;
        if (cor != null) StopCoroutine(cor);
        cor = null;
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
