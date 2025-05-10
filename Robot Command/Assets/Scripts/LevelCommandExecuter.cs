using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCommandExecuter : CommandExecuter
{
    private PlayerController _player;

    public event Action OnLevelReset;

    private void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
        _warningManager = FindFirstObjectByType<WarningManager>();
    }

    protected override IEnumerator ParseCommand(string commandLine)
    {
        int openParen = commandLine.IndexOf('(');
        int closeParen = commandLine.IndexOf(')');

        if (openParen == -1 || closeParen == -1 || closeParen <= openParen)
        {
            throw new FormatException("Скобки не найдены или имеют неправильный порядок.");
        }

        string commandName = commandLine.Substring(0, openParen);
        string argument = commandLine.Substring(openParen + 1, closeParen - openParen - 1);

        return commandName switch
        {
            "Forward" => _player.MoveForwardCoroutine(argument),
            "Fd" => _player.MoveForwardCoroutine(argument),
            "TurnLeft" => _player.TurnCoroutine(argument, false),
            "TurnRight" => _player.TurnCoroutine(argument, true),
            "Lt" => _player.TurnCoroutine(argument, false),
            "Rt" => _player.TurnCoroutine(argument, true),
            "PickUp" => _player.PickUpCoroutine(),
            "Put" => _player.PutCoroutine(argument),
            "Use" => _player.UseCoroutine(argument),
            _ => throw new KeyNotFoundException($"Команда {commandName} не найдена.")
        };
    }

    public void ResetPlayerPosition()
    {
        _inputField.interactable = true;
        _executeButton.interactable = true;

        OnLevelReset?.Invoke();

        StopAllCoroutines();
    }
}
