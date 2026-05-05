using UnityEngine;

public class TaskScript1 : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            taskManager.CompleteTask(task.ID);
        }
    }
}
