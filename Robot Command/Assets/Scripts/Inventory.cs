using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _maxSlots;
    [SerializeField] private GameObject _slotPrefab;
    private GameItem[] _items;
    private Image[] _slots;

    private RectTransform _inventoryPanel;

    private LevelCommandExecuter _executer;

    private void Awake()
    {
        _items = new GameItem[_maxSlots];
        _slots = new Image[_maxSlots];

        _inventoryPanel = GetComponent<RectTransform>();
        Vector2 panelSize = _inventoryPanel.sizeDelta;
        panelSize.x = 140 + 120 * (_maxSlots - 1);
        _inventoryPanel.sizeDelta = panelSize;

        for (int i = 0; i < _maxSlots; i++)
        {
            GameObject slot = Instantiate(_slotPrefab, _inventoryPanel);
            _slots[i] = slot.transform.GetChild(0).GetComponent<Image>();
        }
    }

    private void Start()
    {
        _executer = FindFirstObjectByType<LevelCommandExecuter>();
        _executer.OnLevelReset += ResetInventory;
    }

    private void OnDisable()
    {
        _executer.OnLevelReset -= ResetInventory;
    }

    public void AddItem(GameItem item)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = item;

                Image slotImage = _slots[i];
                slotImage.sprite = item.ItemSprite;

                item.gameObject.SetActive(false);

                return;
            }
        }

        throw new NotImplementedException("Вы пытаетесь добавить предмет в полный инвентарь");
    }

    public GameItem DropItem(int index)
    {
        if (IsSlotEmpty(index)) throw new Exception("Вы пытаетесь выбросить предмет, которого нет"); 

        GameItem item = _items[index];

        _items[index] = null;
        _slots[index].sprite = null;

        return item;
    }

    public bool IsSlotEmpty(int index)
    {
        return _items[index] == null;
    }

    public GameItem[] GetItems()
    {
        return _items;
    }

    private void ResetInventory()
    {
        _items = new GameItem[_maxSlots];

        foreach (Image image in _slots)
        {
            image.sprite = null;
        }
    }
}
