using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

    [field: SerializeField] public float DoorOpenTime = 1;

    public bool DoorIsOpen { get; private set; } = false;

    private LevelCommandExecuter _commandExecuter;

    [SerializeField] private GameItem _rightKey;

    private void Start()
    {
        _commandExecuter = FindFirstObjectByType<LevelCommandExecuter>();

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

    public bool CompareKeys(GameItem key)
    {
        return _rightKey == key;
    }
}
