using UnityEngine;

public class GameItem : MonoBehaviour
{
    private Vector3 _startPos;

    [field: SerializeField] public Sprite ItemSprite { get; private set; }

    private LevelCommandExecuter _executer;

    private void Awake()
    {
        _startPos = transform.position;
    }

    private void Start()
    {
        _executer = FindFirstObjectByType<LevelCommandExecuter>();
        _executer.OnLevelReset += ResetPosition;
    }

    private void OnAplicationQuit()
    {
        _executer.OnLevelReset -= ResetPosition;
    }

    private void ResetPosition()
    {
        transform.position = _startPos;
        gameObject.SetActive(true);
    }
}
