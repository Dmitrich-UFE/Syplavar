using UnityEngine;
using System.Collections;

public class TaskScript24 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset textAtHome;
    [SerializeField] private TextAsset textNearFlower;

    [SerializeField] private GameObject flower;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;


    void OnEnable()
    {
        flower.SetActive(true);

        if (taskManager.Status % 2 == 0)
        {

        }
        else
        {
            vnl.StartPrint(textAtHome);
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
            vnl.StartPrint(textNearFlower);
            cor = StartCoroutine(checkStatus2());
        }
    }
}
