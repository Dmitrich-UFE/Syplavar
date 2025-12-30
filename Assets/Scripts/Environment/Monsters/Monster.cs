using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public string Name { get; protected set; }
    public float Health { get; protected set; }
    
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



}
