using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandExecuter : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _executeButton;

    private Queue<IEnumerator> _commandQueue = new Queue<IEnumerator>();

    private PlayerController _player;

    private void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
    }

    public void ExecuteCommands()
    {
        _inputField.interactable = false;
        _executeButton.interactable = false;

        _commandQueue.Clear();

        string[] lines = _inputField.text.Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            try
            {
                IEnumerator coroutine = ParseCommand(line.Trim());
                if (coroutine != null)
                    _commandQueue.Enqueue(coroutine);
            }
            catch (Exception e)
            {
                Debug.LogError($"������ � ������� \"{line}\": {e.Message}");
            }
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
            throw new FormatException("������ �� ������� ��� ����� ������������ �������.");

        string commandName = commandLine.Substring(0, openParen);
        string argument = commandLine.Substring(openParen + 1, closeParen - openParen - 1);

        return commandName switch
        {
            "Forward" => _player.MoveForwardCoroutine(argument),
            "TurnLeft" => _player.TurnCoroutine(argument, false),
            "TurnRight" => _player.TurnCoroutine(argument, true),
            _ => throw new KeyNotFoundException($"������� {commandName} �� �������.")
        };
    }

    public void ResetPlayerPosition()
    {
        _inputField.interactable = true;
        _executeButton.interactable = true;

        _player.transform.position = Vector3.zero;
        _player.transform.rotation = Quaternion.identity;
    }
}
