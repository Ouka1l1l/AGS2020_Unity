using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemMenu : Menu
{
    [SerializeField]
    private List<TextMeshProUGUI> _itemNames;

    [SerializeField]
    private Image _leftImage;

    [SerializeField]
    private Image _rightImage;

    private float _oldHorizontal;

    private int _indexOffset;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        _offset = 23;
    }

    new private void OnEnable()
    {
        base.OnEnable();

        _oldHorizontal = Input.GetAxis("Horizontal");
        _indexOffset = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var itemList = DungeonManager.instance._player._itemList;

        int textMax = _itemNames.Count;
        int itemMax = itemList.Count;


        if (_indexOffset > 0)
        {
            _leftImage.SetAlpha(0.5f);
            if (horizontal < 0 && _oldHorizontal == 0)
            {
                _indexOffset -= textMax;
            }
        }
        else
        {
            _leftImage.SetAlpha(0.2f);
        }
        if ((_indexOffset / textMax) < ((itemMax - 1) / textMax))
        {
            _rightImage.SetAlpha(0.5f);
            if (horizontal > 0 && _oldHorizontal == 0)
            {
                _indexOffset += textMax;
            }
        }
        else
        {
            _rightImage.SetAlpha(0.2f);
        }

        for(int i = 0; i < textMax; i++)
        {
            if (i + _indexOffset < itemMax)
            {
                _itemNames[i].text = itemList[i + _indexOffset]._name;
            }
            else
            {
                _itemNames[i].text = "";
            }
        }

        Choose((textMax - 1) - ((textMax - 1) - (itemMax % textMax)));

        _oldHorizontal = horizontal;
    }

    protected override void Submit()
    {
        DungeonManager.instance._player.UseItem(_choose + _indexOffset);
        UIManager.instance.CloseMenuAll();
    }
}
