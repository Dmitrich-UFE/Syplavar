using UnityEngine;
using System.Collections;

public class TaskScript2 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;
    void OnEnable()
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

    void OnDisable()
    {
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }
}
