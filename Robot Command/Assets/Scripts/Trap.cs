using System.Collections;
using UnityEngine;

public class Trap : Interactable
{
    [SerializeField] SkinnedMeshRenderer[] _skinnedMeshRenderers;

    [SerializeField] private float _trapOpenTime = 1;

    public bool IsOpened { get; private set; }

    private bool _startOpenState;

    private CommandExecuter _executer;

    private void Start()
    {
        IsOpened = _skinnedMeshRenderers[0].GetBlendShapeWeight(0) == 100 ? true : false;
        _startOpenState = IsOpened;

        _executer = FindFirstObjectByType<CommandExecuter>();
        _executer.OnLevelReset += ResetTrap;
    }

    private void OnDisable()
    {
        _executer.OnLevelReset -= ResetTrap;
    }

    public override void OnInteract()
    {
        StopAllCoroutines();

        if (!IsOpened) StartCoroutine(TrapOpeningCoroutine());
        else StartCoroutine(TrapClosingCoroutine());
    }

    private IEnumerator TrapOpeningCoroutine()
    {
        IsOpened = true;

        float t = 0f;

        while (t < _trapOpenTime)
        {
            foreach (var renderer in _skinnedMeshRenderers)
            {
                renderer.SetBlendShapeWeight(0, t / _trapOpenTime * 100);
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator TrapClosingCoroutine()
    {
        IsOpened = false;

        float t = _trapOpenTime;

        while (t > 0)
        {
            foreach (var renderer in _skinnedMeshRenderers)
            {
                renderer.SetBlendShapeWeight(0, t / _trapOpenTime * 100);
            }
            t -= Time.deltaTime;
            yield return null;
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
