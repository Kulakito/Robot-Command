using System.Collections;
using UnityEngine;
using TMPro;

public static class TextTyper
{
    public static bool isTyping = false;
    public static IEnumerator TypeText(string te, TMP_Text tmp, float interval = .1f)
    {
        isTyping = true;
        for (int i = 0; i < te.Length; i++)
        {
            tmp.text += te[i];
            yield return new WaitForSeconds(interval);
        }
        isTyping = false;
        yield return null;
    }
}
