using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class Monster : MonoBehaviour
{
    public string Name { get; protected set; }
    public float Health { get; protected set; }

    public float SeekDistance { get; protected set; }
    public float Damage { get; protected set; }
    public float AttackDistance { get; protected set; }
    public NavMeshAgent Agent { get; protected set; }
    
    void Start()
    {

         
    }

    internal void GetDamage(float damage)
    {
        if (Health > 0.00001f)
        {
            Health -=damage;
        }
    }


    internal void TryDeath()
    {
        if (Health < 0.00001f)
            Destroy(this.gameObject);
    }

    void Update()
    {
        float distance = Vector3.Distance(PlayerSeeker.GetPlayerTransform().position, transform.position);

        if (distance <= SeekDistance && distance > AttackDistance)
        {
            // Состояние: Преследование
            Agent.SetDestination(PlayerSeeker.GetPlayerTransform().position);
        }
        else if (distance <= AttackDistance)
        {
            // Состояние: Атака (остановка и выполнение действия)
            Agent.SetDestination(transform.position); 
            Attack();
            Debug.Log("Атакую игрока!");
        }
    }

    void Attack()
    {

    }

}