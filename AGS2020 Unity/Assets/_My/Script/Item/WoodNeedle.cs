using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodNeedle : ThrowingItem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _name = "木のとげ";

        _type = ItemType.WoodNeedle;

        _damageVal = 30;
    }
}
