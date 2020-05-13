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
    }

    public override void Raise(Character character)
    {
        base.Raise(character);
        character.Damage(10);
    }
}
