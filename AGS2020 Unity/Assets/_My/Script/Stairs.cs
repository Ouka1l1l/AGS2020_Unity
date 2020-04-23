using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 階段イベント
/// </summary>
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
