using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingPoint : MonoBehaviour
{
    [SerializeField]
    private QuestionText _questionText;

    [SerializeField]
    private Fade _fade;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlayBGM("拠点");
    }

    private void OnTriggerEnter(Collider other)
    {
        _questionText.gameObject.SetActive(true);

        var player = other.gameObject.GetComponent<HubPlayer>();
        player.Stop();

        StartCoroutine(Departure(player));
        
    }

    private IEnumerator Departure(HubPlayer player)
    {
        bool result = false;

        _questionText.SetQuestionText("ダンジョンに挑戦しますか？");
        yield return StartCoroutine(_questionText.Question(r => result = r));

        if (result)
        {
            SoundManager.instance.StopBGM();
            _fade.FadeOut(() => SceneManager.LoadScene("Dungeon"));
        }
        else
        {
            player.ReturnRoom(Vector3.left);
        }
    }
}
