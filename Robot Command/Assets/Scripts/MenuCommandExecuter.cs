using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCommandExecuter : CommandExecuter
{
    protected override IEnumerator ParseCommand(string commandLine)
    {
        int openParen = commandLine.IndexOf('(');
        int closeParen = commandLine.IndexOf(')');

        if (openParen == -1 || closeParen == -1 || closeParen <= openParen)
        {
            throw new FormatException("������ �� ������� ��� ����� ������������ �������.");
        }

        string commandName = commandLine.Substring(0, openParen);
        string argument = commandLine.Substring(openParen + 1, closeParen - openParen - 1);

        return commandName switch
        {
            "Play" => StartPlaying(),
            _ => throw new KeyNotFoundException($"������� {commandName} �� �������.")
        };
    }

    public IEnumerator StartPlaying()
    {
        SceneManager.LoadScene(1);
        yield return null;
    }
}
