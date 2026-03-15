using UnityEngine;

public class PlayerSeeker : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private static Transform _playerTransform;

    void Start()
    {
        _playerTransform = _player.GetComponent<Transform>();
    }

    internal static Transform GetPlayerTransform()
    {
        return _playerTransform;
    }
}