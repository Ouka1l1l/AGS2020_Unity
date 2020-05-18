using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusMenu : Menu
{
    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private TextMeshProUGUI _expText;

    [SerializeField]
    private Slider _expSlider;

    [SerializeField]
    private TextMeshProUGUI _AtkText;

    [SerializeField]
    private TextMeshProUGUI _DefText;

    // Start is called before the first frame update
    new void Start()
    {
        _expSlider.minValue = 0;

        _levelText.text = "";
        _expText.text = "";
        _AtkText.text = "";
        _DefText.text = "";

    }

    new protected void OnEnable()
    {
        DungeonManager.instance.PauseStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
        {
            UIManager.instance.CloseMenu();
        }

        var player = DungeonManager.instance._player;

        _levelText.text = player._level.ToString();

        _expText.text = player._exp.ToString();

        _expSlider.maxValue = player._nextLevelExp;
        _expSlider.value = player._exp;

        _AtkText.text = player._atk.ToString();

        _DefText.text = player._def.ToString();
    }
}
