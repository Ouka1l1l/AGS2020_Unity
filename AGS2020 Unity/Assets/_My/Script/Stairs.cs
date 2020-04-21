using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : Event
{
    private void Awake()
    {
        _type = EventType.Stairs;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Raise(Character character)
    {
        if (character._type == Character.CharacterType.Player)
        {
            DungeonManager.instance.NextLevel();
        }
    }
}
