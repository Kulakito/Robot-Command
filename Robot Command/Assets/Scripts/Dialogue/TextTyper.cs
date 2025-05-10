using System.Collections;
using UnityEngine;
using TMPro;

public static class TextTyper
{
    public static bool isTyping = false;
    public static IEnumerator TypeText(string te, TMP_Text tmp, float interval = .1f)
    {
        isTyping = true;
        foreach (char c in te)
        {
            tmp.text += c;
            yield return new WaitForSeconds(interval);
        }
        isTyping = false;
        yield return null;
    }
}
