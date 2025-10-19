using System.Collections;
using UnityEngine;

public class Trap : Interactable
{
    [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

    [SerializeField] private float _trapOpenTime = 1;

    public bool IsOpened { get; private set; }

    private bool _startOpenState;

    private LevelCommandExecuter _executer;

    private void Start()
    {
        IsOpened = _skinnedMeshRenderers[0].GetBlendShapeWeight(0) == 100 ? true : false;
        _startOpenState = IsOpened;

        _executer = FindFirstObjectByType<LevelCommandExecuter>();
        _executer.OnLevelReset += ResetTrap;
    }

    private void OnDisable()
    {
        _executer.OnLevelReset -= ResetTrap;
    }

    public override void OnInteract()
    {
        StopAllCoroutines();

        StartCoroutine(TrapBlendShapeCoroutine());
    }

    private IEnumerator TrapBlendShapeCoroutine()
    {
        IsOpened = !IsOpened;

        float t = IsOpened ? 0f : _trapOpenTime;
        float direction = IsOpened ? 1f : -1f;

        while ((IsOpened && t < _trapOpenTime) || (!IsOpened && t > 0f))
        {
            foreach (var renderer in _skinnedMeshRenderers)
            {
                renderer.SetBlendShapeWeight(0, t / _trapOpenTime * 100);
            }
            t += direction * Time.deltaTime;
            yield return null;
        }

        float finalWeight = IsOpened ? 100f : 0f;
        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.SetBlendShapeWeight(0, finalWeight);
        }
    }

    private void ResetTrap()
    {
        foreach (var renderer in _skinnedMeshRenderers)
        {
            renderer.SetBlendShapeWeight(0, _startOpenState ? 100 : 0);
        }

        IsOpened = _startOpenState;
        GetComponent<Collider>().isTrigger = false;
    }
}
