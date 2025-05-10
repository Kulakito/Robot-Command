using UnityEngine;

public class WarningManager : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject warningPrefab;

    public void ShowWarning(string message)
    {
        GameObject warningGO = Instantiate(warningPrefab, container);
        warningGO.GetComponent<WarningItemUI>().Initialize(message);
    }
}