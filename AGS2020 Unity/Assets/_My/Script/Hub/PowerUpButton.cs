using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    [SerializeField]
    private PlayerData.Status _key;

    private Button _button;

    [SerializeField]
    private TextMeshProUGUI _requiredAmounttext;

    [SerializeField]
    private int _requiredAmount = 10;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _requiredAmount = 10;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        //ButtonUpdate();
    }

    public void PowerUp()
    {
        if(_requiredAmount > SaveData.instance._playerData.parts)
        {
            SoundManager.instance.PlaySE("キャンセル");
            return;
        }

        SoundManager.instance.PlaySE("決定");

        SaveData.instance._playerData.upCounts[_key]++;
        SaveData.instance._playerData.parts -= _requiredAmount;

        _requiredAmount *= 2;

        _requiredAmounttext.text = _requiredAmount.ToString();
    }

    public void ButtonUpdate()
    {
        _button.interactable = (_requiredAmount <= SaveData.instance._playerData.parts);
    }
}
