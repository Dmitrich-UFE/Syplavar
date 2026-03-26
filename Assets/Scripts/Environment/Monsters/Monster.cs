using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class Monster : MonoBehaviour
{
    public string Name { get; protected set; }
    public float Health { get; protected set; }

    public float SeekDistance { get; protected set; }
    public float MaxSeekDistance { get; protected set; }
    public float Damage { get; protected set; }
    public float AttackDistance { get; protected set; }
    public NavMeshAgent Agent { get; protected set; }

    public float CoolDownSec { get; protected set; }

    protected bool isAvailableForAttack;
    protected bool isRegedAsBattling;
    protected Coroutine LifeCoroutine;
    
    protected virtual void Start()
    {
        isAvailableForAttack = true;
        LifeCoroutine = StartCoroutine(Life());
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
        {
            StopCoroutine(LifeCoroutine);
            if (isRegedAsBattling) BattleStatusTracker.RemoveMonsterInBattleMode();

            BattleStatusTracker.BattleMode = BattleStatusTracker.MonstersInBattleMode != 0;
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        
    }

    IEnumerator Life()
    {
        while (true)
        {
            float distance = Vector3.Distance(PlayerSeeker.GetPlayerTransform().position, transform.position);

            if (distance <= SeekDistance && distance > AttackDistance)
            {
                // Состояние: Преследование
                Agent.SetDestination(PlayerSeeker.GetPlayerTransform().position);
                
                if (BattleStatusTracker.BattleMode == false) BattleStatusTracker.BattleMode = true;
                if (!isRegedAsBattling) BattleStatusTracker.AddMonsterInBattleMode();
                isRegedAsBattling = true;
            }
            else if (distance <= AttackDistance)
            {
                // Состояние: Атака (остановка и выполнение действия)
                Agent.SetDestination(transform.position); 

                if (isAvailableForAttack)
                {
                    Attack();
                    Invoke("switchisAvailableforAttackToTrue", CoolDownSec);
                    Debug.Log("Атакую игрока!");
                }   
           
            }
            else if (distance > MaxSeekDistance)
            {
                if (isRegedAsBattling) BattleStatusTracker.RemoveMonsterInBattleMode();
                BattleStatusTracker.BattleMode = BattleStatusTracker.MonstersInBattleMode != 0;
                isRegedAsBattling = false;
            }

            //Debug.Log(BattleStatusTracker.MonstersInBattleMode);
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }




    internal abstract void Attack();
    

    void switchisAvailableforAttackToTrue()
    {
        isAvailableForAttack = true;
    }
}