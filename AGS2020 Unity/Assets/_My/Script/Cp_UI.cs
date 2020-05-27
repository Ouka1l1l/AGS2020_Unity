using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cp_UI : UI_Bar
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _Name = "CP";
    }

    // Update is called once per frame
    void Update()
    {
        var player = DungeonManager.instance._player;
        Bar(player._cp, player._cpLimit);
    }
}
