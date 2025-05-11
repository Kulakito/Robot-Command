using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCommandExecuter : CommandExecuter
{
    Animator anim;

    private void Start()
    {
        anim = Camera.main.GetComponent<Animator>();
    }

    public override void ExecuteCommands()
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
                _warningManager.ShowWarning($"Ошибка в команде \"{line}\": {e.Message}");
            }
        }

        StartCoroutine(RunCommandQueue());
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
            "Play" => StartPlaying(),
            "Credits" => ShowCredits(),
            _ => throw new KeyNotFoundException($"Команда {commandName} не найдена.")
        };
    }

    public IEnumerator StartPlaying()
    {
        SceneManager.LoadScene(1);
        yield return null;
    }

    public IEnumerator ShowCredits()
    {
        anim.SetBool("active", !anim.GetBool("active"));
        yield return null;
    }
}
