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

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();

        _requiredAmount = 10;
    }

    public void PowerUp()
    {
        SaveData.instance._playerData.upCounts[_key]++;
        _requiredAmount *= 2;

        _requiredAmounttext.text = _requiredAmount.ToString();

        //ButtonUpdate();
    }

    public void ButtonUpdate()
    {
        _button.interactable = (_requiredAmount <= SaveData.instance._playerData.parts);
    }
}
