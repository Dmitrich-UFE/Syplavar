using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    [Header("Настройки эффекта")]
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private string colorPropertyName = "_FlashColor";

    [Header("Ссылка на спрайт игрока")]
    [Tooltip("Перетащите сюда объект с Renderer")]
    [SerializeField] private Renderer targetRenderer;

    private MaterialPropertyBlock _propBlock;
    private Color _originalColor;
    private Coroutine _flashRoutine;
    private PlayerHealth health;
    private int healthValue;


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
        _propBlock = new MaterialPropertyBlock();
        health = PlayerSeeker.GetPlayerHealth();
        health.OnHealthChanged += GetDamage;
        healthValue = health.Health;

        // Берем базовый цвет из первого объекта в списке
        if (targetRenderer != null)
        {
            _originalColor = targetRenderer.sharedMaterial.HasProperty(colorPropertyName) 
                ? targetRenderer.sharedMaterial.GetColor(colorPropertyName) 
                : Color.white;
        }

        _playerInputActions.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        _playerInputActions.Player.Movement.canceled += context => moveInput = Vector2.zero;
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ApplyColor(_originalColor);
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private void GetDamage()
    {
        if (targetRenderer == null) return;
        
        if (healthValue > health.Health)
        {
            healthValue = health.Health;
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }
        
    }

    IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ApplyColor(Color.Lerp(flashColor, _originalColor, elapsed / duration));
            yield return null;
        }
        ApplyColor(_originalColor);
    }

    private void ApplyColor(Color color)
    {
        if (targetRenderer == null) return;
        targetRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(colorPropertyName, color);
        targetRenderer.SetPropertyBlock(_propBlock);
    }
}
