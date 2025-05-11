using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public sealed class FinalDialogue : TutorDialogueHandler
{
    [SerializeField] UnityEvent ending;

    protected override void Start()
    {
        StartCoroutine(ActivateDialogue());
    }

    protected override IEnumerator ActivateDialogue()
    {
        panel.SetActive(true);
        //isActiveDialogue = true;
        for (int i = 0; i < messages.Length; i++)
        {
            te.text = "";
            charImage.sprite = messages[i].charSprite;
            StartCoroutine(TextTyper.TypeText(messages[i].text, te, .025f));
            yield return new WaitUntil(() => !TextTyper.isTyping && Input.GetKeyDown(KeyCode.Space));
            yield return null;
        }
        //isActiveDialogue = false;
        panel.SetActive(false);
        ending.Invoke();
        yield return null;
    }
}
