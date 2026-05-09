using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class ShowItemName : MonoBehaviour
{
    

    [SerializeField] private InventoryAI _inventoryAI;
    [SerializeField] private Cursor _cursor;
    [SerializeField] private float _delay;
    [SerializeField] private TMP_Text _uiText;
    Coroutine coroutine;
    internal static ShowItemName instance;


    void Awake()
    {
        instance = this;
        _inventoryAI.OnSelectedSlotChanged += OnHotbarSelectionChanged;
        _cursor.OnSelectedItemUsed += OnCurrentObjUsed;
        _uiText.text = "";
    }

    //Смена слота
    private void OnHotbarSelectionChanged(int slotIndex, InventorySlotAI slot)
    {
        if (this.enabled && slot != null && slot.ItemData != null)
        {
            ShowActItemText(slot.ItemData.Name);
        }
    }

    //Использование объектов
    private void OnCurrentObjUsed(IItem item)
    {
        if (this.enabled && item.GameObject != null && item.GameObject.CompareTag("WateringCan"))
        {
            WateringCan waterCan = item.GameObject.GetComponent<WateringCan>();
            ShowActItemText($"Лейка. Осталось использований: {waterCan._waterCapaсity}");
        }

    }

    //Вывод любого текста
    internal void ShowActItemText(string data)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        _uiText.text = data;
        _uiText.color = new Color(_uiText.color.r, _uiText.color.g, _uiText.color.b, 1);

        if (this.isActiveAndEnabled) coroutine = StartCoroutine(hideText());
    }

    IEnumerator hideText()
    {
        for (float i = 1 + _delay; i >= 0; i-= 0.02f)
        {
            if (i <=1)
            {
                _uiText.color = new Color(_uiText.color.r, _uiText.color.g, _uiText.color.b, i);
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield break;
    }

    void OnEnable()
    {
        _inventoryAI.OnSelectedSlotChanged += OnHotbarSelectionChanged;
        _cursor.OnSelectedItemUsed += OnCurrentObjUsed;
    }

    void OnDisable()
    {
        _inventoryAI.OnSelectedSlotChanged -= OnHotbarSelectionChanged;
        _cursor.OnSelectedItemUsed -= OnCurrentObjUsed;
    }
}
