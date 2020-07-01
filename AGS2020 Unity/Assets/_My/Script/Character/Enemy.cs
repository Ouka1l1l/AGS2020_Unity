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
        Slime,
        Max
    }

    /// <summary>
    /// 敵のタイプ
    /// </summary>
    public EnemyType _enemyType { get; protected set; }

    /// <summary>
    /// 検知範囲
    /// </summary>
    protected Vector2Int _detectionRange;

    /// <summary>
    /// アイテムを使う
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int _ItemUsePercentage = 50;

    /// <summary>
    /// 攻撃する確率
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int _attackPercentage = 80;

    /// <summary>
    /// 移動する確率
    /// </summary>
    [SerializeField]
    [Range(0,100)]
    private int _movePercentage = 90;

    /// <summary>
    /// 別の部屋に行く確率
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int _anotherRoomPercentage = 50;
    
    /// <summary>
    /// レンダー
    /// </summary>
    protected Renderer[] _renderers;

    /// <summary>
    /// プレイヤー
    /// </summary>
    protected Player _player;

    protected Vector3 _targetPos;

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();

        _type = CharacterType.Enemy;

        _thinkEnd = false;

        _renderers = GetComponentsInChildren<Renderer>();

        var enemyStatus = Resources.Load<EnemyData>("ScriptableObject/EnemyData").enemyData[(int)_enemyType];
        _name = "<color=#0000ff>" + enemyStatus.name + "</color>";
        _hp = _maxHp = enemyStatus.maxHp;
        _atk = enemyStatus.atk;
        _def = enemyStatus.def;
        _exp = enemyStatus.exp;

        _detectionRange = new Vector2Int(3, 3);

        _player = _dungeonManager._player;
    }

    private void Update()
    {
        bool ret = _player.VisibilityCheck(transform.position);
        foreach (var renderer in _renderers)
        {
            renderer.enabled = ret;
        }
    }

    public override bool Think()
    {
        if(_thinkEnd)
        {
            return true;
        }

        if (_itam != null)
        {
            if (Random.Range(0, 100) < _ItemUsePercentage)
            {
                if (CheckUseItem())
                {
                    //アイテムを使った
                    return false;
                }
            }
        }

        if (Random.Range(0, 100) < _attackPercentage)
        {
            if (PlayerDetection(new Vector2Int(1, 1)))
            {
                //攻撃
                _dir = GetTargetDir(_player._destination);
                SetActFunc(AttackAction);
                return false;
            }
        }

        if (Random.Range(0, 100) < _movePercentage)
        {
            MoveThink();
        }

        _thinkEnd = true;
        return false;
    }

    /// <summary>
    /// 移動するか
    /// </summary>
    /// <returns> 移動したか</returns>
    protected bool MoveThink()
    {
        //プレイヤーが周辺にいるか
        if (PlayerDetection(_detectionRange))
        {
            //プレイヤーが周辺にいる
            _targetPos = _player.transform.position;
            if (TowardsTarget())
            {
                //プレイヤーを追いかける
                return true;
            }
        }
        else
        {
            if ((_roomNo == _player._roomNo) && (_roomNo != -1) && (_player._roomNo != -1))
            {
                //プレイヤーが同じ部屋にいる
                _targetPos = _player.transform.position;
                if (TowardsTarget())
                {
                    //プレイヤーを追いかける
                    return true;
                }
            }
            else
            {
                //プレイヤーが違う部屋にいる

                if(GoingToAnotherRoom())
                {
                    return true;
                }

                //ランダム移動
                int d = Random.Range(0, _dir.Max());
                if (SetDestination((Dir)(d * _dir.One())))
                {
                    _targetPos = _destination;
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 別の部屋に行く
    /// </summary>
    /// <returns> 移動したか</returns>
    protected bool GoingToAnotherRoom()
    {
        //今いる場所が道か
        if (_dungeonManager._floor.GetTerrainData(transform.position.x, transform.position.z) == Floor.TerrainType.Road)
        {
            //今いる場所が道
            Vector2Int vec2Int = GetFrontPosition();
            _targetPos = new Vector3(vec2Int.x, transform.position.y, vec2Int.y);
            if (TowardsTarget())
            {
                return true;
            }
        }
        else
        {
            if (_targetPos != transform.position)
            {
                //目的地まで到達した
                if (TowardsTarget())
                {
                    return true;
                }
            }

            if (Random.Range(0, 100) < _anotherRoomPercentage)
            {
                //別の部屋へ
                var roadStartList = _dungeonManager._floor._sections[_roomNo]._roadStartList;

                _targetPos = roadStartList[Random.Range(0, roadStartList.Count)];
                if (TowardsTarget())
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// プレイヤー検知
    /// </summary>
    /// <param name="detectionRange"> 検知範囲</param>
    /// <returns> 検知したか</returns>
    protected bool PlayerDetection(Vector2Int detectionRange)
    {
        var SurroundingCharacterData = _dungeonManager._floor.GetSurroundingCharacterData(transform.position.x, transform.position.z, detectionRange.x, detectionRange.y);

        bool ret = false;
        foreach(var characterID in SurroundingCharacterData.Values)
        {
            if(characterID == _player._id)
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    /// <summary>
    /// ターゲットに近づく
    /// </summary>
    /// <returns> 移動したか</returns>
    protected bool TowardsTarget()
    {
        var dir = GetTargetDir(_targetPos);

        if(SetDestination(dir))
        {
            return true;
        }
        else
        {
            Dir da = dir;
            Dir ds = dir;
            do
            {
                da = da.Addition();
                if(SetDestination(da))
                {
                    return true;
                }

                ds = ds.Subtraction();
                if(SetDestination(ds))
                {
                    return true;
                }

            } while (da != ds);
        }

        return false;
    }

    /// <summary>
    /// アイテムが使えるかチェック
    /// </summary>
    /// <returns> アイテムを使うか</returns>
    protected bool CheckUseItem()
    {
        bool useFlag = false;
        switch (_itam._type)
        {
            case Item.ItemType.MedicalBox:
                if (_hp < (_maxHp / 2))
                {
                    useFlag = true;
                }
                break;

            case Item.ItemType.CP_RecoveryAgents:
                if (_cp > 0)
                {
                    useFlag = true;
                }
                break;

            default:
                Debug.LogError("敵アイテムエラー" + _itam._type);
                break;
        }

        if (useFlag)
        {
            SetActFunc(ItemAction);
            return true;
        }

        return false;
    }

    public void Spawn(int level,int id)
    {
        _id = id;
        _level = level;

        base.Spawn();

        _targetPos = transform.position;
    }

    protected override void Death()
    {
        base.Death();

        _dungeonManager._floor.SetCharacterData(transform.position.x, transform.position.z, -1);

        if(_itam != null)
        {
            _itam.Drop(transform.position.x, transform.position.z);
        }

        Destroy(gameObject);
    }
}
