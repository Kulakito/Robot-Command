using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _maxSlots;
    [SerializeField] private GameObject _slotPrefab;
    private GameItem[] _items;
    private Image[] _slots;

    private RectTransform _inventoryPanel;

    private CommandExecuter _executer;

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
        _executer = FindFirstObjectByType<CommandExecuter>();
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
