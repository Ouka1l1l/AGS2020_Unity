using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrap : Event
{
    [SerializeField]
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

        SoundManager.instance.PlaySE("モンスタートラップ");

        dungeonManager._floor.SpawnEnemiesAround(transform.position, _range);

        dungeonManager._floor.SetTerrainData((int)transform.position.x, (int)transform.position.z, Floor.TerrainType.Floor);

        Destroy(gameObject);
    }
}
