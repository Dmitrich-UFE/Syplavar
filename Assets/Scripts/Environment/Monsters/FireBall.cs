using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private int damage;
    private float lifeTime;

    private float timer;

    public void Init(Vector3 dir, float spd, int dmg, float life)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        lifeTime = life;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHealth>(out var player))
        {
            player.Health -= damage;
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}