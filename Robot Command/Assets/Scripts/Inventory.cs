using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _maxSlots;
    [SerializeField] private GameObject _slotPrefab;

    private RectTransform _inventoryPanel;

    private void Awake()
    {
        _inventoryPanel = GetComponent<RectTransform>();
        Vector2 panelSize = _inventoryPanel.sizeDelta;
        panelSize.x = 140 + 120 * (_maxSlots - 1);
        _inventoryPanel.sizeDelta = panelSize;

        for (int i = 0; i < _maxSlots; i++)
        {
            Instantiate(_slotPrefab, _inventoryPanel);
        }
    }
}
