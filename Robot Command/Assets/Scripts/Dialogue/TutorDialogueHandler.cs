using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorDialogueHandler : MonoBehaviour
{
    [SerializeField] string[] messages;

    [SerializeField] 

    int isViewed = 0;

    void Start()
    {
        isViewed = PlayerPrefs.GetInt($"TutorOnScene{SceneManager.GetActiveScene().buildIndex}Viewed");
        if (isViewed == 0)
        {
            StartCoroutine(ActivateDialogue());
        }
    }

    public IEnumerator ActivateDialogue()
    {
        yield return null;
    }
}
