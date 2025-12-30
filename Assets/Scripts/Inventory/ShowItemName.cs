using UnityEngine;
using TMPro;
using System.Collections;

public class ShowItemName : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] private StaticInventoryDisplay _hotbar;
    [SerializeField] private Cursor _cursor;
    [SerializeField] private float _delay;
    [SerializeField] private TMP_Text _uiText;
    Coroutine coroutine;

    void Awake()
    {
        _hotbar.OnSelectedSlotChanged += OnHotbarSelectionChanged;
        _cursor.OnSelectedItemUsed += OnCurrentObjUsed;
        _uiText.text = "";
    }

    //Смена слота
    private void  OnHotbarSelectionChanged(int slotIndex, InventorySlot slot)
    {
        if (slot != null && slot.ItemData != null)
        {
            ShowActItemText(slot.ItemData.Name);
        }
    }

    //Использование объектов
    private void OnCurrentObjUsed(IItem item)
    {
        if (item.GameObject.CompareTag("WateringCan"))
        {
            WateringCan waterCan = item.GameObject.GetComponent<WateringCan>();
            ShowActItemText($"Лейка. Осталось использований: {waterCan._waterCapaсity}");
        }

    }

    //Вывод любого текста
    private void ShowActItemText(string data)
    {
        if (coroutine != null)
                StopCoroutine(coroutine);

        _uiText.text = data;
        _uiText.color = new Color(_uiText.color.r, _uiText.color.g, _uiText.color.b, 1);

        coroutine = StartCoroutine(hideText());
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
}
