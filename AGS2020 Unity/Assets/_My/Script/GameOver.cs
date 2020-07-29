using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        DungeonManager.instance.PauseStart();

        _text.text = string.Format("{0:d}階まで到達した", DungeonManager.instance._hierarchy);

        StartCoroutine(GameOverFunc());
    }

    public IEnumerator GameOverFunc()
    {
        while(!Input.GetButtonDown("Submit"))
        {
            yield return null;
        }

        StartCoroutine(ReStart());
    }

    public IEnumerator ReStart()
    {
        DungeonManager.instance.PauseStart();

        bool result = false;

        var question = UIManager.instance.Question("再挑戦しますか?").Question(r => result = r);

        yield return StartCoroutine(question);

        if (result)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            GameQuit();
        }

        DungeonManager.instance.PauseEnd();
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
