using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP_RecoveryAgents : Item
{
    private int healValue;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _type = ItemType.CP_RecoveryAgents;
        _name = "CP回復薬";
        healValue = 30;
    }

    public override void Use(Character character)
    {
        base.Use(character);

        character.CpSub(healValue);

        Destroy(gameObject);
    }
}
