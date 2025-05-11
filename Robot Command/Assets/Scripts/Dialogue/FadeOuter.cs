using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FadeOuter : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Image[] images;
    [SerializeField] TMP_Text[] texts;
    bool isGoing = false;
    float goida = 0f;

    void Update()
    {
        if (!isGoing)
            return;
        goida += Time.deltaTime;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, goida);
        }
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, goida);
        }

    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void FadeOut()
    {
        panel.SetActive(true);
        isGoing = true;
    }
}
