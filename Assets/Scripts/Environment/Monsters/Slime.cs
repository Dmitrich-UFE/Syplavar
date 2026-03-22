using UnityEngine;
using UnityEngine.AI;

public class Slime: Monster
{
    [SerializeField] private string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _damage;
    [SerializeField] private float _seekDist;
    [SerializeField] private float _attkDist;
    [SerializeField] private float _coolDownSec;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private PlayerHealth playerHealth;


    void Awake()
    {
        Name = _name;
        Health = _health;
        SeekDistance = _seekDist;
        Damage = _damage;
        AttackDistance = _attkDist;
        CoolDownSec = _coolDownSec;

        playerHealth = PlayerSeeker.GetPlayerHealth();

        agent = GetComponent<NavMeshAgent>();
        Agent = agent;
    }

    internal override void Attack()
    {
        playerHealth.Health -= Damage;
        isAvailableForAttack = false;
    }

    
    //private void OnTriggerEnter(Collider playerObj)
    //{
    //    if (playerObj.CompareTag("Player"))
    //    {
    //        playerHealth = playerHealth.GetComponent<PlayerHealth>();
    //    }
    //}

    //private void OnTriggerExit(Collider playerObj)
    //{
    //    if (playerObj.CompareTag("Player"))
    //    {
    //        playerHealth = null;
    //    }
    //}
}
