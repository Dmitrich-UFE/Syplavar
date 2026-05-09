using UnityEngine;
using System.Collections;

public class TaskScript26 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset textAtHome;
    [SerializeField] private TextAsset textNearFlower;

    [SerializeField] private GameObject flower;
    [SerializeField] private GameObject monsters;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;


    void OnEnable()
    {
        flower.SetActive(true);
        monsters.SetActive(true);

        
        vnl.StartPrint(textAtHome);
        cor = StartCoroutine(checkStatus());
        
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
