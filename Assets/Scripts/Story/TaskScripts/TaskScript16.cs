using UnityEngine;
using System.Collections;

public class TaskScript16 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject DayLightH;
    [SerializeField] private GameObject Monster;
    [SerializeField] private Bed bed;

    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(1f);
    Coroutine cor;

    void OnEnable()
    {
        bed.BlockSleep();
        DayLightHandler.SetDayProgress(0.01f);
        DayLightH.SetActive(false);
        Monster.SetActive(true);

        cor = StartCoroutine(checkStatus());
    }

    IEnumerator checkStatus()
    {
        while (Monster.activeSelf)
        {
            yield return tick;
        }

        taskManager.CompleteTask(task.ID);
    }
}
