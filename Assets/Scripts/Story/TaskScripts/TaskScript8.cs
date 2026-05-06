using UnityEngine;
using System.Collections;

public class TaskScript8 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text1;
    [SerializeField] private TextAsset text2;
    [SerializeField] private BoxCollider boxCollider;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        boxCollider.enabled = false;
        vnl.StartPrint(text1);
        cor = StartCoroutine(checkStatus());

        if (taskManager.Status % 2 == 0)
        {
            boxCollider.enabled = true;
        }
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        boxCollider.enabled = true;
        taskManager.Status *= 2;
    }

    void OnDisable()
    {
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            vnl.StartPrint(text2);
            boxCollider.enabled = false;
            cor = StartCoroutine(checkStatus2());
        }
    }

    IEnumerator checkStatus2()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.CompleteTask(task.ID);
    }
}
