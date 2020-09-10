using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFloor : Event
{
    private ParticleSystem _explosion;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _type = EventType.ExplosionFloor;
        _name = "爆発トラップ";

        _explosion = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    public override void Execution(Character character)
    {
        base.Execution(character);

        if (DungeonManager.instance._player._roomNo == _roomNo)
        {
            SoundManager.instance.PlaySE("爆発");
            _explosion.Play();
        }

        var dungeonManager = DungeonManager.instance;
        var charDataList = dungeonManager._floor.GetSurroundingCharacterData(transform.position.x, transform.position.z, 1, 1);
        foreach(var charData in charDataList)
        {
            var characterNo = charData.Value;
            if (characterNo == 0)
            {
                dungeonManager._player.Damage(10);
            }
            if(characterNo > 0)
            {
                dungeonManager._floor._enemies[characterNo - 1].Damage(10);
            }

        }
    }
}
