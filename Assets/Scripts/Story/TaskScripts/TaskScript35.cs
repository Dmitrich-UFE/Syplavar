using UnityEngine;
using System.Collections;

public class TaskScript35 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private UIHandler inventoryUI;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (inventoryUI != null) inventoryUI.CloseBigInventory();
        if (taskManager.Status % 2 == 0) 
        {
            
        }
        else
        {
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
    }

    void OnDisable()
    {
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            taskManager.CompleteTask(task.ID);
        }
    }
}
