using UnityEngine;

internal class Cursor : MonoBehaviour
{
    [SerializeField] private ItemData _CurrentItem;
    [SerializeField] private StaticInventoryDisplay _hotbar;

    internal IItem CurrentItem { get; private set; }
    internal IInteractable interactableObject {get; private set;}
    [SerializeField] private Transform Archor;
    [SerializeField] private InventoryHolder _inventoryHolder;
    private Transform thisTransform;
    private PlayerInputActions _playerInputActions;

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
        if (interactableObject == null)
        {
            this.interactableObject = unplowedLand;
            interactableObject = unplowedLand;
        }

        //Debug.Log($"взаимодействие с {interactableObject} чем? {CurrentItem.Name}");
        var val = interactableObject.Interact(CurrentItem);

        if (val.isDebitNeed)
        {
            // Списать предмет, если вернул true.
            _hotbar.UseSelectItem();
        }

        if (val.gettingItems != null && val.gettingItems.Count > 0)
        {
            foreach (var item in val.gettingItems)
            {
                //закинуть по предмету в инвентарь
                _inventoryHolder.InventorySystem.AddToInventory((ItemData)item, 1);
            }


        }
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
    private void OnHotbarSelectionChanged(int slotIndex, InventorySlot slot)
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
        CurrentItem = _CurrentItem;

        _playerInputActions = new PlayerInputActions();
        thisTransform = GetComponent<Transform>();
        _playerInputActions.Player.Interact.performed += context => InteractWith(interactableObject);

        _hotbar.OnSelectedSlotChanged += OnHotbarSelectionChanged;

        InventorySlot slot = _hotbar.GetSelectedSlot();
        if (slot != null && slot.ItemData != null)
        {
            SetItem(slot.ItemData);
        }
    }

    void Update()
    {
        //SetPosition();
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