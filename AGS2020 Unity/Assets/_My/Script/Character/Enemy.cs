using System.Collections;
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
    /// 思考終了フラグ
    /// </summary>
    protected bool _thinkEnd;

    /// <summary>
    /// 検知範囲
    /// </summary>
    protected Vector2Int _detectionRange;

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

        if(_action != Action.Non)
        {
            ThinkEnd();
            return true;
        }

        if(_idleHash != _animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            return false;
        }

        if (_itam != null)
        {
            if (Random.Range(0, 100) < 20)
            {
                if (UseItem())
                {
                    return false;
                }
            }
        }

        if (Random.Range(0, 100) > 10)
        {
            if (PlayerDetection(new Vector2Int(1, 1)))
            {
                //攻撃
                _dir = GetTargetDir(_player.transform.position);
                Attack();
                return false;
            }
        }

        if (Random.Range(0, 100) > 20)
        {
            if (PlayerDetection(_detectionRange))
            {
                _targetPos = _player.transform.position;
                //プレイヤーが周辺にいる
                if (TowardsTarget())
                {
                    return false;
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
                        return false;
                    }
                }
                else
                {
                    if (_dungeonManager._floor.GetTerrainData(transform.position.x, transform.position.z) == Floor.TerrainType.Road)
                    {
                        //道の途中
                        Vector2Int vec2Int = GetFrontPosition();
                        _targetPos = new Vector3(vec2Int.x, transform.position.y, vec2Int.y);
                        if (TowardsTarget())
                        {
                            return false;
                        }
                    }
                    else
                    {

                        if (_targetPos != transform.position)
                        {
                            //目的地まで到達した
                            if (TowardsTarget())
                            {
                                return false;
                            }
                        }

                        if (Random.Range(0, 100) < 5)
                        {
                            //別の部屋へ
                            var roadStartList = _dungeonManager._floor._sections[_roomNo]._roadStartList;

                            _targetPos = roadStartList[Random.Range(0, roadStartList.Count)];
                            if (TowardsTarget())
                            {
                                return false;
                            }
                        }
                    }

                    //ランダム移動
                    int d = Random.Range(0, _dir.Max());
                    if (SetDestination((Dir)(d * _dir.One())))
                    {
                        _targetPos = _destination;
                        return false;
                    }
                }
            }
        }

        _action = Action.Wait;
        return false;
    }

    protected void ThinkEnd()
    {
        _thinkEnd = true;
    }

    /// <summary>
    /// プレイヤー検知
    /// </summary>
    /// <param name="detectionRange"></param> 検知範囲
    /// <returns></returns> true 検知した
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

    new public bool Act()
    {
        if(base.Act())
        {
            ActEnd();
            return true;
        }

        return false;
    }

    private void ActEnd()
    {
        _thinkEnd = false;
    }

    new protected bool UseItem()
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
            base.UseItem();
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
