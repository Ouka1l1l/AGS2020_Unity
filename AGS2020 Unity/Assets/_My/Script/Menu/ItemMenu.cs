using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemMenu : BaseMenu
{
    [SerializeField]
    private Button _itemMenuButton;

    [SerializeField]
    private List<Button> _itemButtons;

    [SerializeField]
    private List<TextMeshProUGUI> _itemNames;

    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Button _nextButton;

    private int _indexOffset;

    [SerializeField]
    private Button _exceptionButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        _indexOffset = 0;
    }

    protected override void OnDisable()
    {
        _itemMenuButton.Select();

        base.OnDisable();
    }

    public override void Init()
    {
        ItemTextUpdate();

        if (0 >=DungeonManager.instance._player._itemList.Count)
        {
            _exceptionButton.Select();
            return;
        }

        base.Init();
    }

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
            _itemButtons[i].targetGraphic.enabled = flag;
            _itemButtons[i].enabled = flag;
            if (flag)
            {
                _itemNames[i].text = itemList[i + _indexOffset]._name;
            }
            else
            {
                _itemNames[i].text = "";
            }
        }

        bool interactable = (_indexOffset > 0);
        if(_backButton.enabled != interactable)
        {
            _backButton.enabled = interactable;
        }

        interactable = ((_indexOffset / textMax) < ((itemMax - 1) / textMax));
        if(_nextButton.enabled != interactable)
        {
            _nextButton.enabled = interactable;
        }
    }
}
