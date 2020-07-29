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
    private Button _yesButton;

    [SerializeField]
    private Button _noButton;

    private bool _yes;

    private bool _isSubmit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        _isSubmit = false;
        _yes = false;
    }

    public IEnumerator Question(Action<bool> CallBack)
    {
        _noButton.Select();

        //決定されるまで
        while (!_isSubmit)
        {
            yield return null;
        }

        Selection(CallBack);
    }

    private void Selection(Action<bool> CallBack)
    {
        CallBack(_yes);
        gameObject.SetActive(false);
    }

    public void Submit(bool isYes)
    {
        _isSubmit = true;
        _yes = isYes;
    }

    public void SetQuestionText(string str)
    {
        _question.text = str;
    }
}
