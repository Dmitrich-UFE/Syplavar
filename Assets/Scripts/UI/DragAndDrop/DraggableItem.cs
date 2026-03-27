using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    private Vector3 startPosition;
    private Transform startParent;
    private CanvasGroup canvasGroup;
    internal InventorySlotDragDrop sourceSlot {get; private set;}

    private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

    public void OnBeginDrag(PointerEventData eventData)
    {
        sourceSlot = GetComponentInParent<InventorySlotDragDrop>();
        startPosition = transform.position; // Запоминаем точку возврата
        startParent = transform.parent;
        
        // Чтобы GridLayout не мешал летать, но иерархию не рвем (используем root временно)
        transform.SetParent(transform.root, true); 
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) => transform.position = eventData.position;

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        transform.SetParent(startParent);
        
        // Полный сброс RectTransform
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(48, 48);
        rect.anchoredPosition = Vector2.zero;
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        
    }

}