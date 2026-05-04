using UnityEngine;
using UnityEngine.AI;

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

        agent = GetComponent<NavMeshAgent>();
        Agent = agent;

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

        Invoke("SwitchIsAttackingToFalse", CoolDownSec - 0.5f);
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

    }

    void OnEnable()
    {
        Start();
        base.Start();
    }
}