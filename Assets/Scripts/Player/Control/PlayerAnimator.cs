using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    private PlayerInputActions _playerInputActions;
    private Vector2 _moveInput;

    private Vector2 moveInput 
    {
         get { return _moveInput; } 
         set
         {
            _moveInput = value;
            if (moveInput.magnitude > 0.01f)
            {
                // Обновляем направление только при движении
                playerAnimator.SetFloat("MoveX", moveInput.x);
                playerAnimator.SetFloat("MoveY", moveInput.y);
                playerAnimator.SetFloat("Speed", moveInput.magnitude);
            }
            else
            {
                // Скорость 0 вернет нас в Idle Blend Tree (если оно есть)
                playerAnimator.SetFloat("Speed", 0);
            }
         }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        _playerInputActions.Player.Movement.canceled += context => moveInput = Vector2.zero;
        playerAnimator = GetComponent<Animator>();
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
