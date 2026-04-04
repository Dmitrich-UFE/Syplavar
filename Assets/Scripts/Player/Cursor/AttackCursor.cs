using System.Collections.Generic;
using UnityEngine;

public class AttackCursor : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    internal IInstrument CurrentItem { get; private set; }
    private Transform thisTransform;
    [SerializeField] private InventoryAI _inventoryAI;
    [SerializeField] private SpriteRenderer _cursorSpriteR;

    [SerializeField] private List<Monster> monsters;


    void Awake()
    {
        CurrentItem = null;
        monsters = new List<Monster>();

        _playerInputActions = new PlayerInputActions();
        thisTransform = GetComponent<Transform>();
        _playerInputActions.Player.Attack.performed += context => Attack();

        _inventoryAI.OnSelectedSlotChanged += OnItemChanged;
    }

    void Start()
    {
        InventorySlotAI slot = _inventoryAI.GetActiveItem();
        if (slot != null && slot.ItemData != null)
        {
            SetItem(slot.ItemData);
        }
    }

    private void Attack()
    {
        if (CurrentItem != null)
        {
            List<int> indexForDeleteMonstas = new List<int>();

            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].GetDamage(CurrentItem?.Damage ?? 0);

                if (monsters[i].Health < 0.00001f)
                {
                    indexForDeleteMonstas.Insert(0, i);
                    monsters[i].TryDeath();

                    _cursorSpriteR.color = new Color(1, 0, 0, 1); 
                }
            }

            
            foreach (int i in indexForDeleteMonstas)
            {
                Debug.Log(i);
                monsters.RemoveAt(i);
            }
        }
    }


    private void OnTriggerEnter(Collider monsta)
    {
        if (monsta.CompareTag("Monster"))
        {
            _cursorSpriteR.color = new Color(1, 1, 0, 1); 
            Debug.Log($"столкновение с {monsta.name}");
            monsters.Add(monsta.gameObject.GetComponent<Monster>());
        }
    }

    private void OnTriggerExit(Collider monsta)
    {
        if (monsta.CompareTag("Monster"))
        {
            _cursorSpriteR.color = new Color(1, 0, 0, 1); 
            monsters.Remove(monsta.gameObject.GetComponent<Monster>());
        }
    }

    public void SetItem(ItemData newItem)
    {
        CurrentItem = newItem.GameObject.GetComponent<IInstrument>();
    }

    private void OnItemChanged(int slotIndex, InventorySlotAI slot)
    {
        if (slot.ItemData != null && slot.ItemData.GameObject != null && slot.ItemData.GameObject.GetComponent<IInstrument>() != null)
        {
            SetItem(slot.ItemData);
        }
        else
        {
           CurrentItem = null;
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
