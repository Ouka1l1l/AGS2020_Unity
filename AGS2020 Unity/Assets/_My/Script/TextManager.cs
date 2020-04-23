using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : Singleton<TextManager>
{
    private Queue<string> _texts;

    [SerializeField]
    private TextMeshProUGUI _textMeshPro;

    [SerializeField]
    private int _textDisplayMax = 4;

    // Start is called before the first frame update
    void Start()
    {
        _texts = new Queue<string>();
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

    public void AddText(string str)
    {
        _texts.Enqueue(str);
        if (_texts.Count > _textDisplayMax)
        {
            _texts.Dequeue();
        }
    }
}
