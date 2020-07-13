using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrap : Event
{
    private Vector2Int _range = new Vector2Int(2, 2);

    protected override void Start()
    {
        base.Start();
        _type = EventType.MonsterTrap;
        _name = "モンスタートラップ";
    }

    public override void Execution(Character character)
    {
        var dungeonManager = DungeonManager.instance;

        if (character != dungeonManager._player)
        {
            return;
        }

        base.Execution(character);

        dungeonManager._floor.SpawnEnemiesAround(transform.position, _range);
    }
}
