using UnityEngine;
using TMPro;
using System.Collections;

public class WarningItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private float lifeTime = 2.5f;
    [SerializeField] private CanvasGroup canvasGroup;

    public void Initialize(string message)
    {
        warningText.text = message;
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(lifeTime);

        float fadeTime = 0.5f;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / fadeTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
