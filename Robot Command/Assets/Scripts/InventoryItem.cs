using UnityEngine;

public class InventoryItem
{
    public string Name { get; private set; }
    public Sprite Icon { get; private set; }
    public GameObject ModelPrefab { get; private set; }

    public InventoryItem(string name, Sprite icon, GameObject modelPrefab)
    {
        Name = name;
        Icon = icon;
        ModelPrefab = modelPrefab;
    }
}
