using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestionText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _question;

    [SerializeField]
    private Image _yesPanel;

    [SerializeField]
    private Image _noPanel;

    private bool _yes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        _yes = false;
        _yesPanel.SetAlpha(0);
        _noPanel.SetAlpha(0.5f);
    }

    public IEnumerator Selection(Action<bool> CallBack)
    {
        yield return null;

        //決定されるまで
        while (!Input.GetButtonDown("Submit"))
        {
            if(Input.GetButtonDown("Cancel"))
            {
                _yes = false;
                Submit(CallBack);
                yield break;
            }

            var h = Input.GetAxis("Horizontal");

            if (h < 0)
            {
                _yes = true;
            }
            if (h > 0)
            {
                _yes = false;
            }

            if (_yes)
            {
                _yesPanel.SetAlpha(0.5f);
                _noPanel.SetAlpha(0);
            }
            else
            {
                _yesPanel.SetAlpha(0);
                _noPanel.SetAlpha(0.5f);
            }

            yield return null;
        }

        Submit(CallBack);
    }

    private void Submit(Action<bool> CallBack)
    {
        CallBack(_yes);
        gameObject.SetActive(false);
    }

    public void SetQuestionText(string str)
    {
        _question.text = str;
    }
}
