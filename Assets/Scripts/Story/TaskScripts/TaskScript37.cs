using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.Collections;

public class TaskScript37 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject[] Monsters;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;
    bool isAllMonstersAlive = true;

    void OnEnable()
    {
        foreach (GameObject monsta in Monsters)
        {
            monsta.SetActive(true);
        }

        if (taskManager.Status % 2 == 0) 
        {
            cor = StartCoroutine(checkMonsters());
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
        cor = StartCoroutine(checkMonsters());
    }

    IEnumerator checkMonsters()
    {
        while (isAllMonstersAlive)
        {
            isAllMonstersAlive = Monsters.Any(x => x.activeSelf);
            yield return tick;
        }
        taskManager.CompleteTask(task.ID);
    }

}
