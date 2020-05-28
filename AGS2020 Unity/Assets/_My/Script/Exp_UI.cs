using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Exp_UI : UI_Bar
{
    [SerializeField]
    private TextMeshProUGUI _levelText;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _Name = "";
    }

    // Update is called once per frame
    void Update()
    {
        var player = DungeonManager.instance._player;

        _levelText.text = player._level.ToString();
        base.Bar(player._exp, player._nextLevelExp);
    }
}
