using UnityEngine;
using System.Collections;

public class Task3Main : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;
    
    

    private int count;
    internal int CountOfExploredPlaces 
    {
        get => count;
        set 
        {
            count = value;
            if (count >= 4)
            {
                taskManager.CompleteTask(task.ID);
            }
        } 
    }

    void OnEnable()
    {
        CountOfExploredPlaces = 0;
    }

    

    


}
