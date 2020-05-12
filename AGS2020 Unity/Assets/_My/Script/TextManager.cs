using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextManager : Singleton<TextManager>
{
    /// <summary>
    /// テキストキュー
    /// </summary>
    private Queue<string> _texts;

    /// <summary>
    /// テキスト表示コンポーネント
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _textMeshPro;

    /// <summary>
    /// 最大テキスト表示行
    /// </summary>
    [SerializeField]
    private int _textDisplayMax = 4;

    [SerializeField]
    private QuestionText _QuestionPanel;

    new private void Awake()
    {
        base.Awake();

        _texts = new Queue<string>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _textMeshPro.text = "";
        foreach (var str in _texts)
        {
            _textMeshPro.text += str + '\n';

        }
    }

    /// <summary>
    /// テキストを追加
    /// </summary>
    /// <param name="str"></param> 追加する文字列
    public void AddText(string str)
    {
        _texts.Enqueue(str);
        if (_texts.Count > _textDisplayMax)
        {
            _texts.Dequeue();
        }
    }

    public QuestionText NextLevelText()
    {
        _QuestionPanel.gameObject.SetActive(true);
        _QuestionPanel.SetQuestionText("次の階に進みますか?");
        return _QuestionPanel;
    }
}
