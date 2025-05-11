using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class TutorDialogueHandler : MonoBehaviour
{
    [SerializeField] protected DialogueMessage[] messages;
    [SerializeField] bool activateOnStart = true;

    [SerializeField] protected TMP_Text te;
    [SerializeField] protected GameObject panel;
    [SerializeField] protected Image charImage;

    //bool isActiveDialogue = false;

    protected virtual void Start()
    {
        if (PlayerPrefs.GetInt($"TutorOnScene{SceneManager.GetActiveScene().buildIndex}Viewed") == 0 && activateOnStart)
        {
            StartCoroutine(ActivateDialogue());
            PlayerPrefs.SetInt($"TutorOnScene{SceneManager.GetActiveScene().buildIndex}Viewed", 1);
        }
    }

    public void PlayDialogue()
    {
        StartCoroutine(ActivateDialogue());
    }

    protected virtual IEnumerator ActivateDialogue()
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
        yield return null;
    }
}
[System.Serializable]
public class DialogueMessage
{
    public Sprite charSprite;
    public string text;
}