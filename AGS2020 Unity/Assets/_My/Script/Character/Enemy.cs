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
        Zombie,
        Skeleton,
        Gargoyle,
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
    /// スキル攻撃する確率
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int _skillAttackPercentage = 80;

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
    /// 1ターンでのcp減少値
    /// </summary>
    [SerializeField]
    private int _cpDecreaseValue = 1;

    /// <summary>
    /// レンダー
    /// </summary>
    protected Renderer[] _renderers;

    /// <summary>
    /// 使えるスキル攻撃
    /// </summary>
    private List<SkillAttackType> _skillAttacks;

    /// <summary>
    /// プレイヤー
    /// </summary>
    protected Player _player;

    /// <summary>
    /// プレイヤーから見えているか
    /// </summary>
    private bool _isVisibility = false;

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
        _skillAttacks = enemyStatus.skillAttacks;

        _detectionRange = new Vector2Int(3, 3);

        _player = _dungeonManager._player;
    }

    private void Update()
    {
        bool _isVisibility = _player.VisibilityCheck(transform.position);
        foreach (var renderer in _renderers)
        {
            renderer.enabled = _isVisibility;
        }
    }

    public override bool Think()
    {
        if(_thinkEnd)
        {
            return true;
        }

        if(!AnimationIdleDetection())
        {
            return false;
        }

        //アイテムを持っているか
        if (_itam != null)
        {
            //アイテムを使うか
            if (Random.Range(0, 100) < _ItemUsePercentage)
            {
                //アイテムを使えるか
                if (CheckUseItem())
                {
                    //アイテムを使った
                    return false;
                }
            }
        }

        //攻撃を行うか
        if (Random.Range(0, 100) < _attackPercentage)
        {
            //スキル攻撃を行うか
            if(Random.Range(0, 100) < _skillAttackPercentage)
            {
                if(SkillAttackChoice())
                {
                    return false;
                }
            }

            if (PlayerDetection(new Vector2Int(1, 1)))
            {
                //通常攻撃
                _dir = GetTargetDir(_player.transform.position);
                SetActFunc(AttackAction);
                return false;
            }
        }

        //移動するか
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
        if (_itam.EnemyWhetherToUse(this))
        {
            SetActFunc(ItemAction);
            return true;
        }

        return false;
    }

    /// <summary>
    /// スキル攻撃を選択
    /// </summary>
    /// <returns> 選択したか</returns>
    private bool SkillAttackChoice()
    {
        //使えるスキル攻撃があるか
        //スキル攻撃がリキャスト中じゃないか
        if((_skillAttacks.Count <= 0) || (_cp > 0))
        {
            return false;
        }

        //攻撃範囲内にプレイヤーがいるスキル攻撃の配列番号リスト
        List<int> availableNo = new List<int>();

        //スキルごとの範囲チェック
        for(int i = 0; i < _skillAttacks.Count;i++)
        {
            var skillAttackRange = GetSkillAttackRange(_skillAttacks[i]);
            foreach(var attackPos in skillAttackRange)
            {
                if(_dungeonManager._floor.GetCharacterData(attackPos) == _player._id)
                {
                    //攻撃範囲内にプレイヤーがいる
                    availableNo.Add(i);
                    break;
                }
            }
        }

        if(availableNo.Count <= 0)
        {
            //攻撃範囲内にプレイヤーがいるスキル攻撃がない
            return false;
        }

        //攻撃範囲内にプレイヤーがいるスキル攻撃の中から
        //ランダムで選択
        int useNo = availableNo[Random.Range(0, availableNo.Count)];
        SetSkillAttackAction(_skillAttacks[useNo]);

        return true;
    }

    protected override int SkillAttack(SkillAttackType skillType)
    {
        var ret = base.SkillAttack(skillType);

        _dir = GetTargetDir(_player.transform.position);
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        return ret;
    }

    protected override void ActEnd()
    {
        CpSub(_cpDecreaseValue);
        base.ActEnd();
    }

    protected override void Init(Vector2Int pos, int roomNo)
    {
        base.Init(pos, roomNo);

        _targetPos = transform.position;
    }

    private IEnumerator Appearance()
    {
        DungeonManager.instance.PauseStart();

        transform.SetY(5.0f);

        Vector3 ground = transform.position;
        ground.y = 0.0f;

        while (transform.position.y > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, ground, Time.deltaTime * 7.0f);
            transform.Rotate(Vector3.up, 5);
            yield return null;
        }

        transform.SetY(0.0f);
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        DungeonManager.instance.PauseEnd();
    }

    /// <summary>
    /// 座標を指定してスポーン
    /// </summary>
    /// <param name="x"> X座標</param>
    /// <param name="z"> Z座標</param>
    /// <param name="level"> 敵のレベル</param>
    /// <param name="id"> キャラ番号</param>
    public void SpawnSetPosition(int x, int z, int level, int id)
    {
        _id = id;
        _level = level;

        int roomNo = DungeonManager.instance._floor.GetRoomNo(x, z);

        Init(new Vector2Int(x, z), roomNo);

        StartCoroutine(Appearance());
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

        _dungeonManager._floor.SetCharacterData(transform.position.x, transform.position.z, -1);

        if(_itam != null)
        {
            _itam.Drop(transform.position.x, transform.position.z);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// アニメーションからPlaySEを呼び出す用
    /// </summary>
    /// <param name="seName"> SEの名前</param>
    protected override void PlaySe(string seName)
    {
        if (!_isVisibility)
        {
            base.PlaySe(seName);
        }
    }
}
