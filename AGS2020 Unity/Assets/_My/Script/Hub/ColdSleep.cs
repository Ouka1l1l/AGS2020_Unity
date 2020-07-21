using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColdSleep : MonoBehaviour
{
    [SerializeField]
    private QuestionText _questionText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        _questionText.gameObject.SetActive(true);

        var player = other.gameObject.GetComponent<HubPlayer>();
        player.Stop();

        StartCoroutine(Sleep(player));
    }

    private IEnumerator Sleep(HubPlayer player)
    {
        bool result = false;

        _questionText.SetQuestionText("タイトルに戻りますか？");
        yield return StartCoroutine(_questionText.Selection(r => result = r));

        if (result)
        {
            SceneManager.LoadScene("Title");
        }
        else
        {
            player.ReturnRoom(Vector3.back);
        }
    }
}
