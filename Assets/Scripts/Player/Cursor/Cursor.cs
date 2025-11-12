using UnityEngine;

internal class Cursor : MonoBehaviour
{
    
    internal IItem CurrentItem { get; private set; }
    internal IInteractable interactableObject {get; private set;}
    [SerializeField] private Transform Archor;
    private Transform thisTransform;

    public void SetItem(ref IItem newItem)
    {
        CurrentItem = newItem;
    }
   
    //если интерактивный объект будет null, то имеет смысл присваивать свойству объект голой земли через ??
    void InteractWith(ref IInteractable interactableObject)
    {
        //закомментил для проверки передвижения персонажа, выдавало тут ошибку
        //interactableObject?.Interact(ref CurrentItem);
    }
    
    private void OnTriggerEnter(Collider interactableObject)
    {
        this.interactableObject = interactableObject.gameObject.GetComponent<IInteractable>();
    }

    private void SetPosition()
    {
        thisTransform.position = new Vector3(Mathf.Round(Archor.position.x), thisTransform.position.y, Mathf.Round(Archor.position.z));
        this.interactableObject = null;
    }


    void Awake()
    {
        thisTransform = GetComponent<Transform>();
    }

    void Update()
    {
        SetPosition();
        
            //закомментил для проверки передвижения персонажа, выдавало тут ошибку
            //InteractWith(ref interactableObject);
        
    }
}