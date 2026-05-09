using UnityEngine;
using System.Collections;

public class TaskScript42 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject BigFlower;
    [SerializeField] private PlantTree bigFlowerTree;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 2 == 0) 
        {
            cor = StartCoroutine(checkFlower());
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
        cor = StartCoroutine(checkFlower());
    }

    IEnumerator checkFlower()
    {
        bigFlowerTree.enabled = true;

        while (BigFlower != null)
        {
            yield return tick;
        }
        
        taskManager.CompleteTask(task.ID);
    }
}
