using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float startHealth;
    [SerializeField] private float maxHealth;

    private float _health;
    internal float Health 
    {
        get
        {
            return _health;
        }
        set
        {
            if (value <= 0) Debug.LogError("The health is less or equal zero!!!"); 
            if (value > maxHealth) _health = maxHealth;
            _health = value;
        }
    }

    void Awake() { Health = startHealth;}

    bool TryDeath()
    {
        if (Health <= 0)
        {
            return true;
        }

        return false;
    }






}
