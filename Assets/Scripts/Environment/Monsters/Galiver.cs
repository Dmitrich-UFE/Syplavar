using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class Galiver : Monster
{
    [SerializeField] private string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private float _seekDist;
    [SerializeField] private float _maxseekDist;
    [SerializeField] private float _attkDist;
    [SerializeField] private float _coolDownSec;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform playerTarget;

    [SerializeField] private bool destroyAfterDeath;

    [SerializeField] private Animator galiverAnimator;

    [Header("Fireball")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireballSpeed = 8f;
    [SerializeField] private float fireballLifeTime = 3f;

    [Header("Дроп с монстров")]
    [SerializeField] private ItemData[] returningItems;
    [SerializeField] private int percentsForDropItem;
    [SerializeField] private InventoryAI inventoryAI;
     

    private Coroutine cor;
    private WaitForSecondsRealtime tickGal = new WaitForSecondsRealtime(0.1f);

    protected override void Start()
    {
        Name = _name;
        Health = _health;
        Speed = _speed;
        SeekDistance = _seekDist;
        MaxSeekDistance = _maxseekDist;
        Damage = _damage;
        AttackDistance = _attkDist;
        CoolDownSec = _coolDownSec;

        playerHealth = PlayerSeeker.GetPlayerHealth();
        playerTarget = PlayerSeeker.GetPlayerTransform();
        inventoryAI = PlayerSeeker.GetPlayerInventoryAI();

        agent = GetComponent<NavMeshAgent>();
        Agent = agent;
        cor = StartCoroutine(AnimGaliver());

        base.Start();
    }

    internal override void Attack()
    {
        if (fireballPrefab == null || firePoint == null) return;

        Vector3 playerTargetPos = new Vector3(playerTarget.position.x, playerTarget.position.y + 1f, playerTarget.position.z);
        Vector3 dir = (playerTargetPos - firePoint.position).normalized;

        GameObject fb = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        FireBall fireBall = fb.GetComponent<FireBall>();
        if (fireBall != null)
        {
            fireBall.Init(dir, fireballSpeed, Damage, fireballLifeTime);
        }

        galiverAnimator.SetBool("IsAttacking", true);

        isAvailableForAttack = false;

        Invoke("SwitchIsAttackingToFalse", 0.9f);
    }

    internal override void TryDeath()
    {
        if (Health < 0.00001f)
        {
            StopCoroutine(LifeCoroutine);
            if (AttackCoroutine != null) StopCoroutine(AttackCoroutine);
            if (PeaceCoroutine != null) StopCoroutine(PeaceCoroutine);
            if (isRegedAsBattling) BattleStatusTracker.RemoveMonsterInBattleMode();

            BattleStatusTracker.BattleMode = BattleStatusTracker.MonstersInBattleMode != 0;

            galiverAnimator.SetBool("IsDeath", true);

            //Дроп
            foreach (var item in returningItems)
            {
                int helpInt = UnityEngine.Random.Range(0, 100);
                if (helpInt < percentsForDropItem)
                {
                    inventoryAI.AddToInventory(item, 1);
                }
            }

            if (destroyAfterDeath) Invoke("DestroyThis", 1f);
            else Invoke("SetOff", 1f);
        }
    }

    void SwitchIsAttackingToFalse()
    {
        galiverAnimator.SetBool("IsAttacking", false);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }

    void SetOff()
    {
        gameObject.SetActive(false);

        galiverAnimator.SetBool("IsDeath", false);
        SwitchIsAttackingToFalse();
    }

    void OnDisable()
    {
        if (isRegedAsBattling) BattleStatusTracker.RemoveMonsterInBattleMode();

        BattleStatusTracker.BattleMode = BattleStatusTracker.MonstersInBattleMode != 0;
        StopCoroutine(cor);
    }


    void OnEnable()
    {
        Start();
    }

    IEnumerator AnimGaliver()
    {
        while (true)
        {
            Vector3 direction = Agent.velocity.normalized;

            float forwardSpeed = direction.z; // Скорость вперед/назад
            float sideSpeed = direction.x;    // Скорость влево/вправо
            float speed = forwardSpeed + sideSpeed;
            galiverAnimator.SetFloat("Speed", speed);

            if (Math.Abs(speed) > 0f)
            {
                galiverAnimator.SetFloat("MoveX", sideSpeed);
                galiverAnimator.SetFloat("MoveY", forwardSpeed);
            }

            yield return tickGal;
        }

    }
}