using UnityEngine;
using UnityEngine.AI;

public class Slime: Monster
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
    [SerializeField] private bool destroyAfterDeath;
    [SerializeField] private Animator slimeBodyAnimator;
    [SerializeField] private Animator slimeGroundAnimator;


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

        agent = GetComponent<NavMeshAgent>();
        Agent = agent;

        base.Start();
    }

    internal override void Attack()
    {
        playerHealth.Health -= Damage;
        slimeBodyAnimator.SetBool("IsAttacking", true);
        slimeGroundAnimator.SetBool("IsAttacking", true);
        isAvailableForAttack = false;
        Invoke("switchIsAttackingToFalse", CoolDownSec - 1.25f);
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
            slimeBodyAnimator.SetBool("IsDeath", true);
            slimeGroundAnimator.SetBool("IsDeath", true);

            if (destroyAfterDeath) Invoke("DestroyThis", 1f);
            else 
            {
                Invoke("SetOff", 1f);
            }
        }
    }

    void switchIsAttackingToFalse()
    {
        slimeBodyAnimator.SetBool("IsAttacking", false);
        slimeGroundAnimator.SetBool("IsAttacking", false);
    }

    void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    void SetOff()
    {
        gameObject.SetActive(false);
        slimeBodyAnimator.SetBool("IsDeath", false);
        slimeGroundAnimator.SetBool("IsDeath", false);
        switchIsAttackingToFalse();
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
