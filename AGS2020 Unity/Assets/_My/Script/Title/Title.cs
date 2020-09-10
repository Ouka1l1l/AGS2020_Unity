using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField]
    private Button _beginButton;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private Fade _fade;

    // Start is called before the first frame update
    void Start()
    {
        _fade.FadeIn(() => _beginButton.Select());

        SoundManager.instance.PlayBGM("タイトル");

        if (!SaveData.instance.FileCheck())
        {
            _continueButton.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetButtonDown("Submit"))
        //{
        //    SceneManager.LoadScene("Hub");
        //}
    }

    public void BeginButton()
    {
        SaveData.instance.CreateNewData();
        SceneTransition();
    }

    public void ContinueButton()
    {
        SaveData.instance.Load();
        SceneTransition();
    }

    private void SceneTransition()
    {
        SoundManager.instance.PlaySE("決定");

        SoundManager.instance.StopBGM();

        _fade.FadeOut(() => SceneManager.LoadScene("Hub"));
    }
}
