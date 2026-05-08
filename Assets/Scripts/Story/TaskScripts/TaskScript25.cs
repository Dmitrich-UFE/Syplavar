using UnityEngine;
using System.Collections;

public class TaskScript25 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private GameObject flower;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        flower.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
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
