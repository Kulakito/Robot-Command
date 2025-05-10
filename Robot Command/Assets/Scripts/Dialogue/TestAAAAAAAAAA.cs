using UnityEngine;
using TMPro;

public class TestAAAAAAAAAA : MonoBehaviour
{
    public TMP_Text aaa;

    public void aaaaaaaaaaaa(string a)
    {
        if (TextTyper.isTyping)
            return;
        aaa.text = "";
        StartCoroutine(TextTyper.TypeText(a, aaa));
    }
}
