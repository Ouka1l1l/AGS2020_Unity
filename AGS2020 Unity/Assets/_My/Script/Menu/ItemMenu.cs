using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemMenu : BaseMenu
{
    [SerializeField]
    private List<Button> _itemButtons;

    [SerializeField]
    private List<TextMeshProUGUI> _itemNames;

    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Button _nextButton;

    private int _indexOffset;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var name in _itemNames)
        {
            name.text = "";
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _indexOffset = 0;
    }

    public override void Init()
    {
        base.Init();
        ItemTextUpdate();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    var itemList = DungeonManager.instance._player._itemList;

    //    int textMax = _itemNames.Count;
    //    int itemMax = itemList.Count;


    //    if (_indexOffset > 0)
    //    {
    //        _leftImage.SetAlpha(0.5f);
    //        if (horizontal < 0 && _oldHorizontal == 0)
    //        {
    //            _indexOffset -= textMax;
    //        }
    //    }
    //    else
    //    {
    //        _leftImage.SetAlpha(0.2f);
    //    }
    //    if ((_indexOffset / textMax) < ((itemMax - 1) / textMax))
    //    {
    //        _rightImage.SetAlpha(0.5f);
    //        if (horizontal > 0 && _oldHorizontal == 0)
    //        {
    //            _indexOffset += textMax;
    //        }
    //    }
    //    else
    //    {
    //        _rightImage.SetAlpha(0.2f);
    //    }

    //    for (int i = 0; i < textMax; i++)
    //    {
    //        if (i + _indexOffset < itemMax)
    //        {
    //            _itemNames[i].text = itemList[i + _indexOffset]._name;
    //        }
    //        else
    //        {
    //            _itemNames[i].text = "";
    //        }
    //    }

    //    if (_indexOffset + textMax < itemMax)
    //    {
    //        Choose(textMax);
    //    }
    //    else
    //    {
    //        int headlineMax = itemMax % textMax;

    //        if (headlineMax == 0 && itemMax > 0)
    //        {
    //            Choose(textMax);
    //        }
    //        else
    //        {
    //            Choose(itemMax % textMax);
    //        }
    //    }

    //    _oldHorizontal = horizontal;
    //}

    public void Submit(int no)
    {
        DungeonManager.instance._player.UseItem(no + _indexOffset);
        UIManager.instance.CloseMenuAll();
    }

    public void Select(bool isNext)
    {
        if (isNext)
        {
            _indexOffset += _itemNames.Count;
        }
        else
        {
            _indexOffset -= _itemNames.Count;
        }
        ItemTextUpdate();

        _startSelectButton.Select();
    }

    private void ItemTextUpdate()
    {
        var itemList = DungeonManager.instance._player._itemList;

        int textMax = _itemNames.Count;
        int itemMax = itemList.Count;

        for (int i = 0; i < textMax; i++)
        {
            bool flag = (i + _indexOffset < itemMax);
            _itemButtons[i].interactable = flag;
            if (flag)
            {
                _itemNames[i].text = itemList[i + _indexOffset]._name;
            }
            else
            {
                _itemNames[i].text = "";
            }
        }

        _backButton.interactable = (_indexOffset > 0);
        _nextButton.interactable = ((_indexOffset / textMax) < ((itemMax - 1) / textMax));
    }
}
