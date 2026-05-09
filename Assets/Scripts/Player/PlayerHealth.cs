using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startHealth;
    [SerializeField] private int maxHealth;

    private int _health;
    public event System.Action OnHealthChanged;

    internal bool isDeath { get; private set; }
    internal int Health 
    {
        get
        {
            return _health;
        }
        set
        {
            int newHealth = Mathf.Clamp(value, 0, maxHealth);
        
            if (newHealth == _health) return;

            _health = newHealth;

            OnHealthChanged?.Invoke();

            if (_health <= 0 && !isDeath) 
            {
                isDeath = TryDeath();
                Debug.Log("Character has died.");
            }
        }
    }

    internal float HealthInPercents
    {
        get 
        {
            return Health / (float)maxHealth;
        }
    }

    internal void ResetHealth()
    {
        Health = maxHealth;
    }

    void Awake() 
    { 
        Health = startHealth;
    }

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
