using Unity.VisualScripting;
using UnityEngine;

internal class Cursor : MonoBehaviour
{
    [SerializeField] private ItemData CurrentItem;
    [SerializeField] private InventoryAI _inventoryAI;
    private PlayerHealth _playerHealth;
    //private PlayerMind _playerMind;
    internal IInteractable interactableObject {get; private set;}
    [SerializeField] private Transform Archor;
    private Transform thisTransform;
    private PlayerInputActions _playerInputActions;

    public event System.Action<IItem> OnSelectedItemUsed;

    //голая земля
    [SerializeField] GameObject unplowedLand_GameObject;
    internal IInteractable unplowedLand {get; private set;}
    [SerializeField] private SpriteRenderer _cursorSpriteR;


    public void SetItem(ItemData newItem)
    {
        CurrentItem = newItem;
    }
   
    //если интерактивный объект будет null, то имеет смысл присваивать свойству объект голой земли через ??
    void InteractWith(IInteractable interactableObject)
    {
        if (interactableObject == null || interactableObject.Equals(null))
        {
            this.interactableObject = unplowedLand;
            interactableObject = unplowedLand;
        }

        //Debug.Log($"взаимодействие с {interactableObject} чем? {CurrentItem.Name}");
        //if (_inventoryAI.CheckCountOfItem(CurrentItem) < 1) return;
        
        var val = interactableObject.Interact(CurrentItem);
        OnSelectedItemUsed?.Invoke(CurrentItem);

        if (val.isDebitNeed)
        {
            // Списать предмет, если вернул true.
            _inventoryAI.UseActiveItem();
        }

        if (val.gettingItems != null && val.gettingItems.Count > 0)
        {
            foreach (var item in val.gettingItems)
            {
                //закинуть по предмету в инвентарь
                _inventoryAI.AddToInventory((ItemData)item, 1);
            }
        }

        //if (interactableObject.IsUnityNull() || ReferenceEquals(interactableObject, null) || interactableObject == null)
        //{
        //    _cursorSpriteR.color = new Color(1, 0, 0, 1); 
        //}
    }
    
    private void OnTriggerEnter(Collider interactableObject)
    {
        if (interactableObject.CompareTag("InteractableObject"))
        {
            _cursorSpriteR.color = new Color(1, 1, 1, 1); 
            Debug.Log($"столкновение с {interactableObject.gameObject.name}");
            this.interactableObject = interactableObject.gameObject.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit(Collider interactableObject)
    {
        if (interactableObject.CompareTag("InteractableObject"))
        {
            _cursorSpriteR.color = new Color(1, 0, 0, 1); 
            this.interactableObject = null;
        }
    }

    private void SetPosition()
    {
        thisTransform.position = new Vector3(Mathf.Round(Archor.position.x), thisTransform.position.y, Mathf.Round(Archor.position.z));
    }

    // Обработка события изменения выбранного слота
    private void OnHotbarSelectionChanged(int slotIndex, InventorySlotAI slot)
    {
        if (slot != null && slot.ItemData != null)
        {
            SetItem(slot.ItemData);
        }
        else
        {
            SetItem(null);
        }
    }

    void Awake()
    {
        unplowedLand = unplowedLand_GameObject.GetComponent<IInteractable>();

        _playerInputActions = new PlayerInputActions();
        thisTransform = GetComponent<Transform>();
        _playerInputActions.Player.Interact.performed += context => InteractWith(interactableObject);
        _playerInputActions.Player.EatFood.performed += context => EatFood();

        _inventoryAI.OnSelectedSlotChanged += OnHotbarSelectionChanged;

        _playerHealth = PlayerSeeker.GetPlayerHealth();
    }

    void EatFood()
    {
        ItemDataFood _food = CurrentItem as ItemDataFood;
        if (_food != null)
        {
            _playerHealth.Health += _food.AddingHealth;
            //_playerMind.AddMind(_food.AddingMind);
            _inventoryAI.UseActiveItem();
        }
    }

    void Start()
    {
        InventorySlotAI slot = _inventoryAI.GetActiveItem();
        if (slot != null && slot.ItemData != null)
        {
            SetItem(slot.ItemData);
        }
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