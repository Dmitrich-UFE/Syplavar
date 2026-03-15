using UnityEngine;
using UnityEngine.AI;

public class Slime: Monster
{
    [SerializeField] private string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _seekDist;
    [SerializeField] private float _attkDist;
    [SerializeField] private NavMeshAgent agent;


    void Awake()
    {
        Name = _name;
        Health = _health;
        SeekDistance = _seekDist;
        Damage = _damage;
        AttackDistance = _attkDist;

        agent = GetComponent<NavMeshAgent>();
        Agent = agent;
    }




    
}
