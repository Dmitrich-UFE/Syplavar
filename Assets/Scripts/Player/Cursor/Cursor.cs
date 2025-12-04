using UnityEngine;

internal class Cursor : MonoBehaviour
{
    [SerializeField] private ItemData _CurrentItem;
    
    internal IItem CurrentItem { get; private set; }
    internal IInteractable interactableObject {get; private set;}
    [SerializeField] private Transform Archor;
    [SerializeField] private InventoryHolder _inventoryHolder;
    private Transform thisTransform;
    private PlayerInputActions _playerInputActions;

    public void SetItem(ref IItem newItem)
    {
        CurrentItem = newItem;
    }
   
    //если интерактивный объект будет null, то имеет смысл присваивать свойству объект голой земли через ??
    void InteractWith(IInteractable interactableObject)
    {
        //Debug.Log("Вызван метод действия с объектом");
        if (interactableObject == null)
        {
             //присвоение объекта голой земли
        }
        
        var val = interactableObject.Interact(CurrentItem);

        if (val.isDebitNeed)
        {
            // Списать предмет, если вернул true.

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
        this.interactableObject = null;

        if (interactableObject.CompareTag("InteractableObject"))
        {
            this.interactableObject = interactableObject.gameObject.GetComponent<IInteractable>();
            Debug.Log($"столкновение с {interactableObject.gameObject.name}");
        }
            
    }

    private void SetPosition()
    {
        thisTransform.position = new Vector3(Mathf.Round(Archor.position.x), thisTransform.position.y, Mathf.Round(Archor.position.z));
    }


    void Awake()
    {
        CurrentItem = _CurrentItem;

        _playerInputActions = new PlayerInputActions();
        thisTransform = GetComponent<Transform>();
        _playerInputActions.Player.Interact.performed += context => InteractWith(interactableObject);
    }

    void Update()
    {
        SetPosition();
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