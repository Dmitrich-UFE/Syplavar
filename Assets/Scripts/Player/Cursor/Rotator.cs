using Unity.Mathematics.Geometry;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private Vector2 _moveInput;
    private Transform thisTransform;

    private void Awake()
    {
        thisTransform = GetComponent<Transform>();
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Movement.performed += context => _moveInput = context.ReadValue<Vector2>();
        //_playerInputActions.Player.Movement.canceled += context => _moveInput = Vector2.zero;
    }

    void Update()
    {
        thisTransform.rotation = Quaternion.Euler(thisTransform.rotation.eulerAngles.x, GetDegrees(_moveInput), thisTransform.rotation.eulerAngles.z);
    }
    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private float GetDegrees(Vector2 vector)
    {
        return Mathf.Atan2(-vector.y, vector.x) * 180 / Mathf.PI;
    }
}
