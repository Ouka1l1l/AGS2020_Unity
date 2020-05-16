﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    /// <summary>
    /// 敵のタイプenum
    /// </summary>
    public enum EnemyType
    {
        TestEnemy,
        Max
    }

    /// <summary>
    /// 敵のタイプ
    /// </summary>
    public EnemyType _enemyType { get; protected set; }

    /// <summary>
    /// 思考終了フラグ
    /// </summary>
    protected bool _thinkEnd;

    /// <summary>
    /// 行動終了フラグ
    /// </summary>
    protected bool _actEnd;

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();

        _type = CharacterType.Enemy;

        _thinkEnd = false;
        _actEnd = false;
    }

    public override bool Think()
    {
        if(_thinkEnd)
        {
            return true;
        }

        if(_action != Action.Wait)
        {
            ThinkEnd();
            return true;
        }

        if (_destination == transform.position)
        {
            var player = DungeonManager.instance._player;

            var characterDatas = DungeonManager.instance._level.GetSurroundingCharacterData(transform.position.x, transform.position.z, 1, 1);
            foreach (var character in characterDatas)
            {
                if (character == 0)
                {
                    //攻撃
                    _dir = GetTargetDir(player.transform.position);
                    Attack();
                    return false;
                }
            }

            if ((_roomNo == player._roomNo) && (_roomNo != -1) && (player._roomNo != -1))
            {
                var dir = GetTargetDir(player.transform.position);
                if (!SetDestination(dir))
                {
                    if (!SetDestination(dir.Addition()))
                    {
                        SetDestination(dir.Subtraction());
                    }
                }

                return false;
            }
            else
            {
                int d = Random.Range(0, _dir.Max());
                if(SetDestination((Dir)(d * _dir.One())))
                {
                    return false;
                }
            }
        }

        return false;
    }

    protected void ThinkEnd()
    {
        _thinkEnd = true;
        _actEnd = false;
    }

    new public bool Act()
    {
        if(_actEnd)
        {
            return true;
        }

        if(base.Act())
        {
            ActEnd();
            return true;
        }

        return false;
    }

    private void ActEnd()
    {
        _actEnd = true;
        _thinkEnd = false;
    }

    public void Spawn(int level,int id)
    {
        _id = id;
        _level = level;

        base.Spawn();
    }

    protected override void Death()
    {
        base.Death();

        var dungeonManager = DungeonManager.instance;
        dungeonManager._level.SetCharacterData(transform.position.x, transform.position.z, -1);

        Destroy(gameObject);
    }
}
