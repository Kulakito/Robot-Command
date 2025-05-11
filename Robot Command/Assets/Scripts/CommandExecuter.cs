using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;

public abstract class CommandExecuter : MonoBehaviour
{
    [SerializeField] protected TMP_InputField _inputField;
    [SerializeField] protected UnityEngine.UI.Button _executeButton;

    protected Queue<IEnumerator> _commandQueue = new Queue<IEnumerator>();

    protected WarningManager _warningManager;

    public virtual void ExecuteCommands()
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

        if (_commandQueue.Count > 0)
        {
            _inputField.interactable = false;
            _executeButton.interactable = false;
        }

        StartCoroutine(RunCommandQueue());  
    }

    protected IEnumerator RunCommandQueue()
    {
        while (_commandQueue.Count > 0)
        {
            yield return StartCoroutine(_commandQueue.Dequeue());
        }
    }

    protected abstract IEnumerator ParseCommand(string commandLine);

}
