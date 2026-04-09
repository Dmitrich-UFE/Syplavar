using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startHealth;
    [SerializeField] private int maxHealth;

    private int _health;

    internal bool isDeath { get; private set; }
    internal int Health 
    {
        get
        {
            return _health;
        }
        set
        {
            if (value <= 0) 
            {
                Debug.LogWarning("The health is less or equal zero!!!"); 
                value = 0;
                isDeath = TryDeath();
            }
            if (value > maxHealth) _health = maxHealth;
            _health = value;
        }
    }

    void Awake() { Health = startHealth;}

    bool TryDeath()
    {
        if (Health <= 0)
        {
            Debug.Log("Чел умер");
            return true;
        }

        return false;
    }

}
