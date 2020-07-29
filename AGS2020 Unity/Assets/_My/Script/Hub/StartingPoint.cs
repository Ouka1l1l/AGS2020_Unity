using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingPoint : MonoBehaviour
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

        StartCoroutine(Departure(player));
        
    }

    private IEnumerator Departure(HubPlayer player)
    {
        bool result = false;

        _questionText.SetQuestionText("ダンジョンに挑戦しますか？");
        yield return StartCoroutine(_questionText.Question(r => result = r));

        if (result)
        {
            SceneManager.LoadScene("Dungeon");
        }
        else
        {
            player.ReturnRoom(Vector3.left);
        }
    }
}
