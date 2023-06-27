using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    public GameObject loading;

    [SerializeField]
    public Slider loadingBar;

    [SerializeField]
    public TextMeshProUGUI loadingText;

    [SerializeField]
    public Animator transition;

    public float transitionTime = 1f;

    public void loadLevel(string levelName)
    {
        Debug.Log("Loading new scene: " + levelName);

        StartCoroutine(loadSceneAsync(levelName));
    }

    IEnumerator loadSceneAsync(string levelName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        AsyncOperation op = SceneManager.LoadSceneAsync(levelName);
        loading.SetActive(true);

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            loadingBar.value = progress;
            loadingText.text = progress * 100f + "%";

            yield return null;
        }
    }
}
