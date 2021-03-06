﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColdSleep : MonoBehaviour
{
    [SerializeField]
    private QuestionText _questionText;

    [SerializeField]
    private GameObject _doorR;

    [SerializeField]
    private GameObject _doorL;

    private Collider _collider;

    [SerializeField]
    private Fade _fade;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();

        _fade.FadeIn();

        StartCoroutine(OpenDoor());
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

        _questionText.SetQuestionText("セーブしますか？");
        yield return StartCoroutine(_questionText.Question(r => result = r));

        if (result)
        {
            SaveData.instance.Save();
        }

        _questionText.gameObject.SetActive(true);

        _questionText.SetQuestionText("タイトルに戻りますか？");
        yield return StartCoroutine(_questionText.Question(r => result = r));

        if (result)
        {
            _fade.FadeOut(() => SceneManager.LoadScene("Title"));
        }
        else
        {
            player.ReturnRoom(Vector3.back);
        }
    }

    private IEnumerator OpenDoor()
    {
        _collider.enabled = false;

        yield return new WaitForSeconds(1);

        SoundManager.instance.PlaySE("ドア");

        for (int i = 0; i < 5; i++)
        {
            var pos = _doorR.transform.position;
            pos.x += 0.1f;
            _doorR.transform.position = pos;

            pos = _doorL.transform.position;
            pos.x -= 0.1f;
            _doorL.transform.position = pos;

            yield return null;
        }

        yield return new WaitForSeconds(2);

        _collider.enabled = true;
    }
}
