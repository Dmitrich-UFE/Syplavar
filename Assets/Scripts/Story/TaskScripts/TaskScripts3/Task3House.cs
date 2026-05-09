using UnityEngine;

public class Task3House : MonoBehaviour
{
    [SerializeField] private Task3Main task3main;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private int multiplyNum;
    bool isVisited = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isVisited && other.gameObject.CompareTag("Player"))
        {
            vnl.StartPrint(text);
            task3main.CountOfExploredPlaces++;
            taskManager.Status *= multiplyNum;
            isVisited = true;
            this.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (taskManager.Status % multiplyNum == 0)
        {
            task3main.CountOfExploredPlaces++;
            isVisited = true;
            this.gameObject.SetActive(false);
        }
    }
}
