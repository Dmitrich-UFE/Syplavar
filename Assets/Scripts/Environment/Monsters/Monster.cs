using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class Monster : MonoBehaviour
{
    public string Name { get; protected set; }
    public float Health { get; protected set; }
    public float Speed { get; protected set; }

    public float SeekDistance { get; protected set; }
    public float MaxSeekDistance { get; protected set; }
    public int Damage { get; protected set; }
    public float AttackDistance { get; protected set; }
    public NavMeshAgent Agent { get; protected set; }

    public float CoolDownSec { get; protected set; }

    protected bool isAvailableForAttack;
    protected bool isRegedAsBattling;

    //Корутины
    protected Coroutine LifeCoroutine;
    protected Coroutine AttackCoroutine;
    protected Coroutine PeaceCoroutine;
    
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

    //Контролирующая корутина для смены режима монстра
    IEnumerator Life()
    {
        PeaceCoroutine = StartCoroutine(PeaceMode());

        while (true)
        {
            float distance = Vector3.Distance(PlayerSeeker.GetPlayerTransform().position, transform.position);

            if (distance <= SeekDistance && !isRegedAsBattling)
            {
                if (PeaceCoroutine != null) StopCoroutine(PeaceCoroutine);
                AttackCoroutine = StartCoroutine(AttackMode());
            }
            else if (distance > MaxSeekDistance && isRegedAsBattling)
            {
                if (AttackCoroutine != null) StopCoroutine(AttackCoroutine);
                PeaceCoroutine = StartCoroutine(PeaceMode());
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    //Корутина для мирного режима
    IEnumerator PeaceMode()
    {
        //Debug.Log("InPeaceMode");

        if (isRegedAsBattling) BattleStatusTracker.RemoveMonsterInBattleMode();
        BattleStatusTracker.BattleMode = BattleStatusTracker.MonstersInBattleMode != 0;
        isRegedAsBattling = false;
        Agent.speed = Speed / 2.5f;

        while (true)
        {
            int randNum = Random.Range(0, 3);

            if (randNum == 0) 
            {
                Vector3 destPos = new Vector3(gameObject.transform.position.x + Random.Range(-MaxSeekDistance, MaxSeekDistance), 
                    gameObject.transform.position.y, gameObject.transform.position.z + Random.Range(-MaxSeekDistance, MaxSeekDistance));

                Agent.SetDestination(destPos); 
            }
            
            yield return new WaitForSecondsRealtime(2f);
        }
    }

    //Корутина для атаки
    IEnumerator AttackMode()
    {
        //Debug.Log("InAtkMode");

        if (BattleStatusTracker.BattleMode == false) BattleStatusTracker.BattleMode = true;
        if (!isRegedAsBattling) BattleStatusTracker.AddMonsterInBattleMode();
        isRegedAsBattling = true;
        Agent.speed = Speed;

        while (true)
        {
            float distance = Vector3.Distance(PlayerSeeker.GetPlayerTransform().position, transform.position);
            Agent.SetDestination(PlayerSeeker.GetPlayerTransform().position);

            if (distance <= AttackDistance)
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

            yield return new WaitForSecondsRealtime(0.3f);
        }
    }


    internal abstract void Attack();
    
    void switchisAvailableforAttackToTrue()
    {
        isAvailableForAttack = true;
    }
}
