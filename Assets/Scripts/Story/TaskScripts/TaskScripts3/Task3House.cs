using UnityEngine;

public class Task3House : MonoBehaviour
{
    [SerializeField] private Task3Main task3main;
    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    [SerializeField] private TaskManager taskManager;
    bool isVisited = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isVisited && other.gameObject.CompareTag("Player"))
        {
            vnl.StartPrint(text);
            task3main.CountOfExploredPlaces++;
            taskManager.Status += 2;
            isVisited = true;
        }
    }
}
