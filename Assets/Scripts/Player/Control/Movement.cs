using UnityEngine;

public class Movement : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private Vector2 _moveInput;
    private Rigidbody _rb;
    private Vector3 _movementDirection;

    [SerializeField] private float Speed = 5f;
    [SerializeField] private float Boost = 2f;

    private float actualSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Movement.performed += context => _moveInput = context.ReadValue<Vector2>();
        _playerInputActions.Player.Movement.canceled += context => _moveInput = Vector2.zero;

        actualSpeed = Speed;

        _playerInputActions.Player.Run.performed += context => actualSpeed = Speed * Boost;
        _playerInputActions.Player.Run.canceled += context => actualSpeed = Speed;
    }

    private void FixedUpdate()
    {
        _movementDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
        _rb.MovePosition(transform.position + _movementDirection * actualSpeed * Time.deltaTime);
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
}