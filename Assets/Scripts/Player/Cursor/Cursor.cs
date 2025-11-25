using UnityEngine;

internal class Cursor : MonoBehaviour
{
    
    internal IItem CurrentItem { get; private set; }
    internal IInteractable interactableObject {get; private set;}
    [SerializeField] private Transform Archor;
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
        if (interactableObject != null)
        {
            if (interactableObject.Interact(CurrentItem))
            {
            //Функция interact выполняется всегда. Списать предмет, если вернул true. 
            }
        }
        else
        {
            //присвоение объекта голой земли

            interactableObject.Interact(CurrentItem);
        }
        
    }
    
    private void OnTriggerEnter(Collider interactableObject)
    {
        if (interactableObject.CompareTag("InteractableObject"))
        {
            this.interactableObject = interactableObject.gameObject.GetComponent<IInteractable>();
            Debug.Log($"столкновение с {interactableObject.gameObject.name}");
        }
            
    }

    private void SetPosition()
    {
        thisTransform.position = new Vector3(Mathf.Round(Archor.position.x), thisTransform.position.y, Mathf.Round(Archor.position.z));
        this.interactableObject = null;
    }


    void Awake()
    {
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