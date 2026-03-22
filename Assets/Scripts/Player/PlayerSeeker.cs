using UnityEngine;

public class PlayerSeeker : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private static Transform _playerTransform;
    private static PlayerHealth _playerHealth;

    void Awake()
    {
        _playerTransform = _player.GetComponent<Transform>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
    }

    internal static Transform GetPlayerTransform()
    {
        return _playerTransform;
    }

    internal static PlayerHealth GetPlayerHealth()
    {
        return _playerHealth;
    }
}