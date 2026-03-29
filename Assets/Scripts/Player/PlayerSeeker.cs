using UnityEngine;

public class PlayerSeeker : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private static Transform _playerTransform;
    private static PlayerHealth _playerHealth;
    private static GameObject _playerStatic;

    void Awake()
    {
        _playerTransform = _player.GetComponent<Transform>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        _playerStatic = _player;
    }

    internal static Transform GetPlayerTransform()
    {
        return _playerTransform;
    }

    internal static PlayerHealth GetPlayerHealth()
    {
        return _playerHealth;
    }

    internal static GameObject GetPlayer()
    {
        return _playerStatic;
    }
}