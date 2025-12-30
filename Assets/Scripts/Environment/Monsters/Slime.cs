using UnityEngine;

public class Slime: Monster
{
    [SerializeField] private string _name;
    [SerializeField] private float _health;




    void Awake()
    {
        Name = _name;
        Health = _health;
    }




    
}
