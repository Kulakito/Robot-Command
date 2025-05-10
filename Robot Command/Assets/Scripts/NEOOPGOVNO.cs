using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NEOOPGOVNO : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _executeButton;

    private Queue<IEnumerator> _commandQueue = new Queue<IEnumerator>();

    //private MenuController _menu;

    public event Action OnLevelReset;

    /*private void Start()
    {
        _menu = FindFirstObjectByType<MenuController>();
    }*/

    public void ExecuteCommands()
    {
        _commandQueue.Clear();

        string[] lines = _inputField.text.Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            try
            {
                IEnumerator coroutine = ParseCommand(line.Trim());
                if (coroutine != null) _commandQueue.Enqueue(coroutine);
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка в команде \"{line}\": {e.Message}");
            }
        }

        if (_commandQueue.Count > 0)
        {
            _inputField.interactable = false;
            _executeButton.interactable = false;
        }

        StartCoroutine(RunCommandQueue());
    }

    private IEnumerator RunCommandQueue()
    {
        while (_commandQueue.Count > 0)
        {
            yield return StartCoroutine(_commandQueue.Dequeue());
        }
    }

    private IEnumerator ParseCommand(string commandLine)
    {
        int openParen = commandLine.IndexOf('(');
        int closeParen = commandLine.IndexOf(')');

        if (openParen == -1 || closeParen == -1 || closeParen <= openParen)
            throw new FormatException("Скобки не найдены или имеют неправильный порядок.");

        string commandName = commandLine.Substring(0, openParen);
        string argument = commandLine.Substring(openParen + 1, closeParen - openParen - 1);

        return commandName switch
        {
            "Play" => StartPlaying(),
            _ => throw new KeyNotFoundException($"Команда {commandName} не найдена.")
        };
    }

    public IEnumerator StartPlaying()
    {
        SceneManager.LoadScene(1);
        yield return null;
    }
}
