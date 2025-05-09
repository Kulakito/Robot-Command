using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cellPassingDuration = 0.5f;
    [SerializeField] private float _rotationDuration = 0.5f;

    [SerializeField] private float _cellsDistance;

    private Inventory _inventory;

    private Rigidbody _rb;
    private Animator _animator;

    private void Start()
    {
        _inventory = FindFirstObjectByType<Inventory>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    public IEnumerator MoveForwardCoroutine(string argument)
    {
        if (!int.TryParse(argument, out int steps))
            throw new FormatException("Аргумент не является числом.");

        Vector3 startPosition = _rb.position;
        Vector3 direction = transform.forward.normalized;
        Vector3 targetPosition = startPosition + direction * steps * _cellsDistance;

        _animator.SetBool("IsMoving", true);

        while (Vector3.Distance(_rb.position, targetPosition) > 0.01f)
        {
            float moveStep = (_cellsDistance / _cellPassingDuration) * Time.fixedDeltaTime;
            Vector3 newPosition = Vector3.MoveTowards(_rb.position, targetPosition, moveStep);
            _rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        _rb.MovePosition(targetPosition);

        _animator.SetBool("IsMoving", false);
    }

    public IEnumerator TurnCoroutine(string argument, bool isRight)
    {
        if (!int.TryParse(argument, out int steps))
            throw new FormatException("Аргумент не является числом.");

        float totalAngle = 90f * steps * (isRight ? 1 : -1);

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + totalAngle, 0);

        float elapsed = 0f;

        while (elapsed < _rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / _rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public IEnumerator PickUpCoroutine()
    {
        float radius = _cellsDistance / 2;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        int gameItemCount = 0;
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out GameItem gameItem))
            {
                gameItemCount++;
                break;
            }
        }

        if (gameItemCount != 0)
        {
            _animator.SetTrigger("PickUp");

            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("pick up"))
            {
                yield return null;
            }

            float animationTime = _animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(animationTime);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out GameItem gameItem))
                {
                    _inventory.AddItem(gameItem);
                }
            }
        }
        else
        {
            throw new Exception("Вы пытаетесь использовать команду PickUp, но на вашей клетке нету предметов");
        }
    }
}
