﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public static class ColorEnumExtension
{
    public static void SetAlpha(this Image Image, float a)
    {
        var color = Image.color;
        Image.color = new Color(color.r, color.g, color.b, a);
    }
}

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
        DungeonManager.instance.PauseStart();
    }

    public IEnumerator Selection(Action<bool> CallBack)
    {
        //決定されるまで
        while (Input.GetAxis("Submit") <= 0)
        {
            if(Input.GetAxis("Cancel") > 0)
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
        DungeonManager.instance.PauseEnd();
        gameObject.SetActive(false);
    }

    public void SetQuestionText(string str)
    {
        _question.text = str;
    }
}