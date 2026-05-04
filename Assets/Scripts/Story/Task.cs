using UnityEngine;


public class Task : MonoBehaviour
{
    [SerializeField] private int _ID;
    [SerializeField] private string _Name;
    [SerializeField] private string _Description;
    [SerializeField] private string _GoalDescription;
    [SerializeField] private GameObject _TaskObject;
    [SerializeField] private bool _isCompleted;


    internal int ID => _ID;
    internal string Name => _Name;
    internal string Description => _Description;
    internal string GoalDescription => _GoalDescription;
    internal GameObject TaskObject => _TaskObject;
    internal bool IsCompleted => _isCompleted;


    internal void Activate()
    {
        _TaskObject.SetActive(true);
    }

    internal void Deactivate()
    {
        _TaskObject.SetActive(false);
    }
    
    internal void Complete()
    {
        Deactivate();
        _isCompleted = true;
    }
}
