using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cellPassingDuration = 0.5f;
    [SerializeField] private float _rotationDuration = 0.5f;

    [SerializeField] private float _cellsDistance;

    private Inventory _inventory;

    private void Start()
    {
        _inventory = FindFirstObjectByType<Inventory>();
    }

    public IEnumerator MoveForwardCoroutine(string argument)
    {
        if (!int.TryParse(argument, out int steps))
            throw new FormatException("Аргумент не является числом.");

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * steps * _cellsDistance;
        float elapsed = 0f;
        float duration = _cellPassingDuration * steps * _cellsDistance;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    public IEnumerator TurnCoroutine(string argument, bool isRight)
    {
        if (!int.TryParse(argument, out int steps))
            throw new FormatException("Аргумент не является числом.");

        float angle = 90f * steps * (isRight ? 1 : -1);
        float rotated = 0f;
        float rotationSpeed = 90 / _rotationDuration;

        while (Mathf.Abs(rotated) < Mathf.Abs(angle))
        {
            float delta = rotationSpeed * Time.deltaTime;
            if (Mathf.Abs(rotated + delta) > Mathf.Abs(angle))
                delta = Mathf.Abs(angle) - Mathf.Abs(rotated);

            delta *= Mathf.Sign(angle);
            transform.Rotate(0, delta, 0);
            rotated += delta;

            yield return null;
        }
    }

    public IEnumerator PickUpCoroutine()
    {
        yield return null;

        float radius = _cellsDistance / 2;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out GameItem gameItem))
            {
                _inventory.AddItem(gameItem);
            }
        }
    }
}
