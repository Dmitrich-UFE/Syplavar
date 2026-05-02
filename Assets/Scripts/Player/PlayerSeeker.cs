using UnityEngine;

public class PlayerSeeker : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerMind _playerMind;
    [SerializeField] private InventoryAI _inventoryAI;
    private static Transform _playerTransformStatic;
    private static PlayerHealth _playerHealthStatic;
    private static GameObject _playerStatic;
    private static PlayerMind _playerMindStatic;
    private static InventoryAI _inventoryAIStatic;

    void Awake()
    {
        _playerTransformStatic = _playerTransform;
        _playerHealthStatic = _playerHealth;
        _playerStatic = _player;
        _playerMindStatic = _playerMind;
        _inventoryAIStatic = _inventoryAI;
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

    internal static InventoryAI GetPlayerInventoryAI()
    {
        return _inventoryAIStatic;
    }
}