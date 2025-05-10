using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cellPassingDuration = 0.5f;
    [SerializeField] private float _rotationDuration = 0.5f;
    [SerializeField] private float _scaleTransformDuration = 1.0f;

    [SerializeField] private float _cellsDistance;

    private Inventory _inventory;

    private Rigidbody _rb;
    private Animator _animator;

    private CommandExecuter _executer;

    [SerializeField] private AnimationCurve _scaleCurve;

    private WarningManager _warningManager;

    private void Start()
    {
        _inventory = FindFirstObjectByType<Inventory>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _executer = FindFirstObjectByType<CommandExecuter>();
        _warningManager = FindFirstObjectByType<WarningManager>();

        _executer.OnLevelReset += ResetPlayer;

        //StartCoroutine(ScaleTransform(Vector3.zero, transform.localScale));
    }

    private void OnDisable()
    {
        _executer.OnLevelReset -= ResetPlayer;
    }

    //public IEnumerator MoveForwardCoroutine(string argument)
    //{
    //    if (!int.TryParse(argument, out int steps))
    //    {
    //        _warningManager.ShowWarning($"Аргумент {argument} не является числом или аргумента вовсе нету.");
    //        throw new FormatException("Аргумент не является числом или аргумента вовсе нету.");
    //    }

    //    Vector3 startPosition = _rb.position;
    //    Vector3 direction = transform.forward.normalized;
    //    Vector3 targetPosition = startPosition + direction * steps * _cellsDistance;

    //    _animator.SetBool("IsMoving", true);

    //    while (Vector3.Distance(_rb.position, targetPosition) > 0.01f)
    //    {
    //        float moveStep = (_cellsDistance / _cellPassingDuration) * Time.fixedDeltaTime;
    //        Vector3 newPosition = Vector3.MoveTowards(_rb.position, targetPosition, moveStep);
    //        _rb.MovePosition(newPosition);

    //        yield return new WaitForFixedUpdate();
    //    }

    //    _rb.MovePosition(targetPosition);

    //    _animator.SetBool("IsMoving", false);

    //    if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
    //    {
    //        if (hit.collider.CompareTag("End")) StartCoroutine(ScaleTransform(transform.localScale, Vector3.zero));
    //    }
    //}

    public IEnumerator MoveForwardCoroutine(string argument)
    {
        if (!int.TryParse(argument, out int steps))
        {
            _warningManager.ShowWarning($"Аргумент {argument} не является числом или аргумента вовсе нету.");
            throw new FormatException("Аргумент не является числом или аргумента вовсе нету.");
        }

        Vector3 direction = transform.forward.normalized;

        _animator.SetBool("IsMoving", true);

        for (int i = 0; i < steps; i++)
        {
            Vector3 targetPosition = _rb.position + direction * _cellsDistance;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitt, _cellsDistance))
            {
                if (hitt.collider.TryGetComponent(out Door door) && !door.DoorIsOpen)
                {
                    _animator.SetBool("IsMoving", false);
                    _executer.StopAllCoroutines();
                    _warningManager.ShowWarning($"Персонаж не может идти в стену");
                    throw new FormatException("Персонаж не может идти в стену");
                }
            }
            while (Vector3.Distance(_rb.position, targetPosition) > 0.01f)
            {
                float moveStep = _cellsDistance / _cellPassingDuration * Time.fixedDeltaTime;
                Vector3 newPosition = Vector3.MoveTowards(_rb.position, targetPosition, moveStep);
                _rb.MovePosition(newPosition);

                yield return new WaitForFixedUpdate();
            }
            _rb.MovePosition(targetPosition);
        }

        _animator.SetBool("IsMoving", false);

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("End")) StartCoroutine(ScaleTransform(transform.localScale, Vector3.zero));
        }
    }

    public IEnumerator TurnCoroutine(string argument, bool isRight)
    {
        if (!int.TryParse(argument, out int steps))
        {
            _warningManager.ShowWarning($"Аргумент {argument} не является числом или аргумента вовсе нету.");
            throw new FormatException("Аргумент не является числом или аргумента вовсе нету.");
        }
            

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
        List<GameItem> items = CheckGameItemsAround();

        if (items.Count != 0)
        {
            _animator.SetTrigger("PickUp");

            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("pick up"))
            {
                yield return null;
            }

            float animationTime = _animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(animationTime);

            foreach (var gameItem in items)
            {
                _inventory.AddItem(gameItem);
            }
        }
        else
        {
            _warningManager.ShowWarning("Вы пытаетесь использовать команду PickUp, но на вашей клетке нету предметов");
            throw new Exception("Вы пытаетесь использовать команду PickUp, но на вашей клетке нету предметов");
        }
    }

    public IEnumerator PutCoroutine(string argument)
    {
        if (!int.TryParse(argument, out int index))
        {
            _warningManager.ShowWarning($"Аргумент {argument} не является числом.");
            throw new FormatException("Аргумент не является числом или аргумента вовсе нету.");
        }
            
        List<GameItem> items = CheckGameItemsAround();

        if (items.Count == 0)
        {
            if (_inventory.IsSlotEmpty(index))
            {
                _warningManager.ShowWarning("Вы пытаетесь выбросить предмет, которого нет");
                throw new Exception("Вы пытаетесь выбросить предмет, которого нет");
            }
                
            _animator.SetTrigger("PickUp");

            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("pick up"))
            {
                yield return null;
            }

            float animationTime = _animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(animationTime);

            GameItem item = _inventory.DropItem(index);
            item.transform.position = new Vector3(UnityEngine.Random.Range(transform.position.x - 0.7f, transform.position.x + 0.7f), item.transform.position.y, UnityEngine.Random.Range(transform.position.z - 0.7f, transform.position.z + 0.7f));
            item.gameObject.SetActive(true);
        }
        else
        {
            _warningManager.ShowWarning("Вы пытаетесь поставить объект на той клетке, где уже есть предмет");
            throw new Exception("Вы пытаетесь поставить объект на той клетке, где уже есть предмет");
        }
    }

    public IEnumerator UseCoroutine(string argument)
    {
        yield return null;

        if (!int.TryParse(argument, out int index))
        {
            _warningManager.ShowWarning($"Аргумент {argument} не является числом или аргумента вовсе нету.");
            throw new FormatException("Аргумент не является числом или аргумента вовсе нету.");
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _cellsDistance))
        {
            if (_inventory.GetItems()[index] == null)
            {
                _warningManager.ShowWarning($"Вы пытаетесь использовать Use, когда у вас нет предмета в слоте");
                throw new FormatException("Вы пытаетесь использовать Use, когда у вас нет предмета в слоте");
            }

            if (_inventory.GetItems()[index].CompareTag("Key") && hit.collider.TryGetComponent(out Door door))
            {
                door.OpenDoor();
                _inventory.DropItem(index);

                yield return new WaitForSeconds(door.DoorOpenTime + 0.5f);
            }
            else
            {
                _warningManager.ShowWarning("Предмет, который вы хотите использовать не является ключом и/или вы пытаетесь открыть не дверь");
                throw new Exception("Предмет, который вы хотите использовать не является ключом и/или вы пытаетесь открыть не дверь");
            }
        }
        else 
        {
            _warningManager.ShowWarning("Вы пытаетесь использовать предмет, который не на что использовать");
            throw new Exception("Вы пытаетесь использовать предмет, который не на что использовать");
        }
    }

    private List<GameItem> CheckGameItemsAround()
    {
        float radius = _cellsDistance / 2;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        List<GameItem> itemList = new();

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out GameItem gameItem))
            {
                itemList.Add(gameItem);
            }
        }

        return itemList;
    }

    private void ResetPlayer()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        _animator.Rebind();
        _animator.Update(0f);
    }


    private IEnumerator ScaleTransform(Vector3 startScale, Vector3 endScale)
    {
        _executer.StopAllCoroutines();

        transform.localScale = startScale;

        float elapsed = 0;
        while (elapsed < _scaleTransformDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, _scaleCurve.Evaluate(elapsed));
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (endScale == Vector3.zero) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);    
    }
}
