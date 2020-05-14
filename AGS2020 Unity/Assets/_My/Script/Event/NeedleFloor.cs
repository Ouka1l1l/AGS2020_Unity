using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleFloor : Event
{
    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        _type = EventType.NeedleFloor;
        _name = "トゲトラップ";
    }

    public override void Execution(Character character)
    {
        base.Execution(character);
        character.Damage(10);
    }
}
