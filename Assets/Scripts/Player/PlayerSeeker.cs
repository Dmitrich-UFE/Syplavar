using UnityEngine;

public class PlayerSeeker : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerMind _playerMind;
    private static Transform _playerTransformStatic;
    private static PlayerHealth _playerHealthStatic;
    private static GameObject _playerStatic;
    private static PlayerMind _playerMindStatic;

    void Awake()
    {
        _playerTransformStatic = _playerTransform;
        _playerHealthStatic = _playerHealth;
        _playerStatic = _player;
        _playerMindStatic = _playerMind;
    }

    internal static Transform GetPlayerTransform()
    {
        return _playerTransformStatic;
    }

    internal static PlayerHealth GetPlayerHealth()
    {
        return _playerHealthStatic;
    }

    internal static GameObject GetPlayer()
    {
        return _playerStatic;
    }

    internal static PlayerMind GetPlayerMind()
    {
        return _playerMindStatic;
    }
}