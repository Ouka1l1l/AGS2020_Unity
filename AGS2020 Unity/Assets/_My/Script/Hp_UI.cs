﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hp_UI : UI_Bar
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _Name = "HP";
    }

    // Update is called once per frame
    void Update()
    {
        var player = DungeonManager.instance._player;

        Bar(player._hp, player._maxHp);
    }
}
