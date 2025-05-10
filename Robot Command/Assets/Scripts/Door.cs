using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

    [field: SerializeField] public float DoorOpenTime = 1;

    public bool DoorIsOpen { get; private set; } = false;

    private CommandExecuter _commandExecuter;

    private void Start()
    {
        _commandExecuter = FindFirstObjectByType<CommandExecuter>();

        _commandExecuter.OnLevelReset += ResetDoor;
    }

    private void OnDisable()
    {
        _commandExecuter.OnLevelReset -= ResetDoor;
    }

    public void OpenDoor()
    {
        StartCoroutine(DoorOpeningCoroutine());
    }

    private IEnumerator DoorOpeningCoroutine()
    {
        float t = 0f;

        while (t < DoorOpenTime)
        {
            foreach (var renderer in _skinnedMeshRenderers)
            {
                renderer.SetBlendShapeWeight(0, t / DoorOpenTime * 100);
            }
            t += Time.deltaTime;
            yield return null;
        }

        DoorIsOpen = true;
    }

    private void ResetDoor()
    {
        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.SetBlendShapeWeight(0, 0);
        }
    }
}
