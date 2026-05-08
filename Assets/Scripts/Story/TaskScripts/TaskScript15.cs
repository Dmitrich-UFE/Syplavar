using UnityEngine;
using System.Collections;

public class TaskScript15 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject DayLightH;
    [SerializeField] private GameObject Monster;
    [SerializeField] private Bed bed;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset textAtHome;
    [SerializeField] private TextAsset textNearMonster;

    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        bed.BlockSleep();
        DayLightHandler.SetDayProgress(0.01f);
        DayLightH.SetActive(false);
        Monster.SetActive(true);

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
            vnl.StartPrint(textNearMonster);
            cor = StartCoroutine(checkStatus2());
        }
    }
}
