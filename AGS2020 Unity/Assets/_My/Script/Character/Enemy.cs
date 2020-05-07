using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    public enum EnemyType
    {
        TestEnemy,
        Max
    }

    public EnemyType _enemyType { get; protected set; }

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();

        _type = CharacterType.Enemy;

        _turnEnd = true;
    }

    // Update is called once per frame
    new protected void Update()
    {
        if (_destination == transform.position)
        {
            var player = DungeonManager.instance._player;

            var characterDatas = DungeonManager.instance._level.GetSurroundingCharacterData(transform.position.x, transform.position.z, 1, 1);
            foreach(var character in characterDatas)
            {
                if(character == 0)
                {
                    //攻撃
                    _dir = GetTargetDir(player.transform.position);
                    Attack();
                    return;
                }
            }

            if ((_roomNo == player._roomNo) && (_roomNo != -1) && (player._roomNo != -1))
            {
                _dir = GetTargetDir(player.transform.position);
                SetDestination(_dir);
            }
            else
            {
                int d = Random.Range(0, _dir.Max());
                SetDestination((Dir)(d * 45));
            }
        }

        base.Update();
    }

    public void Ai()
    {

    }

    public void Spawn(int level,int id)
    {
        _id = id;
        _level = level;

        base.Spawn();
    }
}
