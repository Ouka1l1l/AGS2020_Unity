using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
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
    private Menu _menu;

    [SerializeField]
    private ItemMenu _itemMenu;

    [SerializeField]
    private QuestionText _QuestionPanel;

    private Stack<Menu> _menus;

    new private void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        _texts = new Queue<string>();
        _menus = new Stack<Menu>();
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

    /// <summary>
    /// テキストをクリアする
    /// </summary>
    public void TextClear()
    {
        _texts = new Queue<string>();
    }

    /// <summary>
    /// メニューを開く
    /// </summary>
    public void OpenMenu()
    {
        _menus.Push(_menu);
        _menu.gameObject.SetActive(true);
    }

    /// <summary>
    /// アイテムメニューを開く
    /// </summary>
    public void OpenItemMenu()
    {
        _menus.Push(_itemMenu);
        _itemMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public void CloseMenu()
    {
        _menus.Pop().gameObject.SetActive(false);
        if(_menus.Count > 0)
        {
            _menus.Peek().PauseEnd();
        }
    }

    /// <summary>
    /// メニューをすべて閉じる
    /// </summary>
    public void CloseMenuAll()
    {
        foreach(var menu in _menus)
        {
            menu.gameObject.SetActive(false);
        }
        _menus.Clear();
    }

    public QuestionText Question(string str)
    {
        _QuestionPanel.gameObject.SetActive(true);
        _QuestionPanel.SetQuestionText(str);
        return _QuestionPanel;
    }
}
