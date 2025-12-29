using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollowingMouse : MonoBehaviour
{
    private Vector3 mousePosition;
    private PlayerInputActions _playerInputActions;
    private Transform _cursorPosTransform;
    [SerializeField] private Transform ArchorTransform;

    [Header("Настройки фильтрации")]
    [SerializeField] private LayerMask allowedLayers;

    internal Vector3 cursorPos
    {
        get { return _cursorPosTransform.position; }
        private set 
        {
            if (_cursorPosTransform.position != value && Vector3.Distance(ArchorTransform.position, value) < 5)
            {
                SetCursorPosition(value);
                _cursorPosTransform.position = value; 
            }

            
        }

    }

    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _cursorPosTransform = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.MousePos.performed += CalculateCursorPosition;
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.MousePos.performed -= CalculateCursorPosition;
        _playerInputActions.Disable();
    }

    
    private void CalculateCursorPosition(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());
        RaycastHit hit;
        Vector3 worldPos;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, allowedLayers))
        {
            worldPos = hit.point;
            cursorPos = new Vector3(Mathf.Round(worldPos.x), _cursorPosTransform.position.y, Mathf.Round(worldPos.z));
        }
    }

    private void SetCursorPosition(Vector3 newPos)
    {
        _cursorPosTransform.position = newPos;
    }

   
}
