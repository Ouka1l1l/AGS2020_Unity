using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 方向
/// </summary>
public enum Dir
{
    Top = 0,
    TopRight = 45,
    Right = 90,
    BottomRight = 135,
    Bottom = 180,
    BottomLeft = 225,
    Left = 270,
    TopLeft = 315,
}

public static class DirEnumExtension
{
    public static Vector2Int ToVector2Int(this Dir dir)
    {
        Vector2Int ret = new Vector2Int();
        switch (dir)
        {
            case Dir.Top:
                ret.y++;
                break;

            case Dir.TopRight:
                ret.y++;
                ret.x++;
                break;

            case Dir.Right:
                ret.x++;
                break;

            case Dir.BottomRight:
                ret.y--;
                ret.x++;
                break;

            case Dir.Bottom:
                ret.y--;
                break;

            case Dir.BottomLeft:
                ret.y--;
                ret.x--;
                break;

            case Dir.Left:
                ret.x--;
                break;

            case Dir.TopLeft:
                ret.y++;
                ret.x--;
                break;

            default:
                Debug.LogError("DirToVector2Int" + dir);
                break;
        }

        return ret;
    }

    public static int Max(this Dir dir)
    {
        return System.Enum.GetValues(typeof(Dir)).Length;
    }

    public static int One(this Dir dir)
    {
        return 360 / System.Enum.GetValues(typeof(Dir)).Length;
    }

    public static Dir Addition(this Dir dir)
    {
        int d = (int)dir + 360 / System.Enum.GetValues(typeof(Dir)).Length;
        if(d >= 360)
        {
            d = d - 360;
        }
        return (Dir)d;
    }

    public static Dir Subtraction(this Dir dir)
    {
        int d = (int)dir - 360 / System.Enum.GetValues(typeof(Dir)).Length;
        if (d < 0)
        {
            d = 360 + d;
        }
        return (Dir)d;
    }
}

public abstract class Character : MonoBehaviour
{
    /// <summary>
    /// キャラのタイプ
    /// </summary>
    public enum CharacterType
    {
        Enemy,
        Player,
        Max
    }

    /// <summary>
    /// 自身のキャラタイプ
    /// </summary>
    public CharacterType _type { get; protected set; }

    /// <summary>
    /// 自身のID
    /// </summary>
    public int _id { get; protected set; }

    /// <summary>
    /// キャラの名前
    /// </summary>
    public string _name { get; protected set; }

    /// <summary>
    /// 移動先
    /// </summary>
    public Vector3 _destination { get; protected set; }

    protected bool _thinkEnd;

    /// <summary>
    /// 現在いるの部屋の区画番号 部屋にいない場合は-1
    /// </summary>
    public int _roomNo { get; protected set; }

    /// <summary>
    /// 持っているアイテム
    /// </summary>
    public Item _itam;

    /// <summary>
    /// 自身の向き
    /// </summary>
    public Dir _dir { get; protected set; }

    /// <summary>
    /// 体力
    /// </summary>
    public int _hp { get; protected set; }

    /// <summary>
    /// 体力の最大値
    /// </summary>
    public int _maxHp { get; protected set; } = 100;

    /// <summary>
    /// レベル
    /// </summary>
    public int _level { get; protected set; } = 1;

    /// <summary>
    /// 経験値
    /// </summary>
    public int _exp { get; protected set; } = 0;

    /// <summary>
    /// レベルアップに必要な経験値
    /// </summary>
    public int _nextLevelExp { get; protected set; } = 100;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int _atk { get; protected set; } = 10;

    /// <summary>
    /// 防御力
    /// </summary>
    public int _def { get; protected set; } = 5;

    public int _cp { get; protected set; }

    /// <summary>
    /// 自動回復量
    /// </summary>
    protected int _regeneration;

    public enum SkillAttackType
    {
        Non,
        RotaryAttack,
        HeavyAttack,
        ThrustAttack,
        MowDownAttack
    }

    /// <summary>
    /// 技のデータ
    /// </summary>
    protected List<SkillAttack> _skillAttackData;

    /// <summary>
    /// アニメーター
    /// </summary>
    protected Animator _animator;

    /// <summary>
    /// スキル攻撃エフェクト
    /// </summary>
    private TrailRenderer _trailRenderer;

    /// <summary>
    /// ActFuncをターン中に１階だけ呼び出すようフラグ
    /// </summary>
    private bool _actFuncOnce = false;

    /// <summary>
    /// Actで実行する関数
    /// </summary>
    protected Action ActFunc;

    /// <summary>
    /// SkillAttackActionで実行する関数
    /// </summary>
    protected Func<int> SkillAttackFunc;

    protected DungeonManager _dungeonManager;

    // Start is called before the first frame update
    protected void Start()
    {
        _dungeonManager = DungeonManager.instance;

        _hp = _maxHp;

        _cp = 0;

        _destination = transform.position;
        _dir = Dir.Bottom;
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        _skillAttackData = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;

        _animator = GetComponentInChildren<Animator>();

        _trailRenderer = transform.GetComponentInChildren<TrailRenderer>();
    }

    /// <summary>
    /// アニメーションが待機かどうか
    /// </summary>
    /// <returns> 待機アニメーションか</returns>
    private bool AnimationIdleDetection()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("アーマチュア|Idle") && !_animator.IsInTransition(0);
    }

    /// <summary>
    /// 行動を決定する
    /// </summary>
    /// <returns></returns> true 行動決定
    public abstract bool Think();

    /// <summary>
    /// ActFuncに行動関数をセット
    /// </summary>
    /// <param name="func">　セットする関数</param>
    protected void SetActFunc(Action func)
    {
        ActFunc = func;
        _thinkEnd = true;
    }

    /// <summary>
    /// 行動を実行する
    /// </summary>
    /// <returns> true 行動終了</returns>
    public bool Act()
    {
        if (AnimationIdleDetection())
        {
            if (!_actFuncOnce)
            {
                ActFunc?.Invoke();
                ActFunc = null;
                _actFuncOnce = true;
            }
            else
            {
                ActEnd();

                if (_trailRenderer != null)
                {
                    _trailRenderer.emitting = false;
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 行動終了時の処理
    /// </summary>
    protected virtual void ActEnd()
    {
        _thinkEnd = false;
        _actFuncOnce = false;
    }

    /// <summary>
    /// 正面の座標を取得
    /// </summary>
    /// <returns> 正面の座標</returns>
    protected Vector2Int GetFrontPosition()
    {
        Vector2Int ret = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        ret += _dir.ToVector2Int();

        return ret;
    }

    /// <summary>
    /// 自身から見てターゲットはどの方向にいるか
    /// </summary>
    /// <param name="targetPos"> ターゲットの座標</param>
    /// <returns> 自身から見たターゲットの方向</returns>
    protected Dir GetTargetDir(Vector3 targetPos)
    {
        Vector2Int vec = new Vector2Int((int)(targetPos.x - transform.position.x), (int)(targetPos.z - transform.position.z));

        string str = "";
        if(vec.y > 0)
        {
            str = "Top";
        }
        else if(vec.y < 0)
        {
            str = "Bottom";
        }

        if(vec.x < 0)
        {
            str += "Left";
        }
        else if(vec.x > 0)
        {
            str += "Right";
        }

        return (Dir)System.Enum.Parse(typeof(Dir), str);
    }

    /// <summary>
    /// 移動先の設定
    /// </summary>
    /// <param name="dir"> 移動方向</param>
    protected bool SetDestination(Dir dir)
    {
        _dir = dir;
        Vector2Int tmpDestination = GetFrontPosition();

        var floor = _dungeonManager._floor;

        transform.rotation = Quaternion.Euler(0, (float)dir, 0);
        if (floor.GetTerrainData(tmpDestination) != Floor.TerrainType.Wall)
        {
            if (floor.GetCharacterData(tmpDestination) == -1)
            {
                //移動先を設定
                _destination = new Vector3(tmpDestination.x, _destination.y, tmpDestination.y);

                //キャラの配置データを更新
                floor.SetCharacterData(transform.position.x, transform.position.z, -1);
                floor.SetCharacterData(tmpDestination.x, tmpDestination.y, _id);

                //現在の部屋番号を更新
                _roomNo = _dungeonManager._floor.GetRoomNo(tmpDestination.x, tmpDestination.y);

                _animator.SetBool("MoveFlag", true);

                _thinkEnd = true;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 移動する
    /// </summary>
    public virtual bool Move()
    {
        if(!_animator.GetBool("MoveFlag"))
        {
            return true;
        }

        transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * 5.0f);
        if (_destination == transform.position)
        {
            _animator.SetBool("MoveFlag", false);

            //足元のイベントを実行
            FootExecution();

            return true;
        }

        return false;
    }

    /// <summary>
    /// アイテムを使う
    /// </summary>
    protected void ItemAction()
    {
        _itam.Use(this);
    }

    /// <summary>
    /// 攻撃の行動
    /// </summary>
    protected virtual void AttackAction()
    {
        Attack();
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    /// <returns> 獲得経験値</returns>
    protected int Attack()
    {
        _animator.SetTrigger("AttackTrigger");

        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        UIManager.instance.AddText(_name + "の攻撃");

        Vector2Int frontPos = GetFrontPosition();
        var characterNo = _dungeonManager._floor.GetCharacterData(frontPos);
        if(characterNo != -1)
        {
            Character target;
            if(characterNo == 0)
            {
                target = _dungeonManager._player;
            }
            else
            {
                target = _dungeonManager._floor._enemies[characterNo - 1];
            }
            if(target.Damage(_atk, target._def))
            {
                return target._exp;
            }
        }

        return 0;
    }

    /// <summary>
    /// スキル攻撃の行動
    /// </summary>
    protected virtual void SkillAttackAction()
    {
        SkillAttackFunc();
        SkillAttackFunc = null;
    }

    /// <summary>
    /// このターンの行動をスキル攻撃に決定
    /// </summary>
    /// <param name="skillAttackType"> セットするスキル攻撃のタイプ</param>
    protected void SetSkillAttackAction(SkillAttackType skillAttackType)
    {
        switch (skillAttackType)
        {
            case SkillAttackType.RotaryAttack:
                SkillAttackFunc = RotaryAttack;
                break;

            case SkillAttackType.HeavyAttack:
                SkillAttackFunc = HeavyAttack;
                break;

            case SkillAttackType.ThrustAttack:
                SkillAttackFunc = ThrustAttack;
                break;

            case SkillAttackType.MowDownAttack:
                SkillAttackFunc = MowDownAttack;
                break;

            default:
                Debug.LogError("技選択エラー" + skillAttackType);
                break;
        }

        SetActFunc(SkillAttackAction);
    }

    /// <summary>
    /// スキル攻撃
    /// </summary>
    /// <param name="skillType"> スキル攻撃のタイプ</param>
    /// <param name="attackPosList"> 攻撃範囲</param>
    /// <returns> 獲得経験値</returns>
    protected virtual int SkillAttack(SkillAttackType skillType)
    {
        if (_trailRenderer != null)
        {
            _trailRenderer.emitting = true;
        }

        _thinkEnd = true;

        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        var data = _skillAttackData[(int)skillType];

        UIManager.instance.AddText(_name + "の" + data.name);

        CpAdd(data.cost);

        var attackPosList = GetSkillAttackRange(skillType);

        int ret = 0;
        foreach (var attackPos in attackPosList)
        {
            var characterNo = _dungeonManager._floor.GetCharacterData(attackPos);
            if (characterNo != -1)
            {
                Character target;
                if (characterNo == 0)
                {
                    target = _dungeonManager._player;
                }
                else
                {
                    if(_id != 0)
                    {
                        continue;
                    }

                    target = _dungeonManager._floor._enemies[characterNo - 1];
                }

                if (target.Damage(_atk + data.addAtk, target._def))
                {
                    ret += target._exp;
                }
            }
        }

        return ret;
    }

    /// <summary>
    /// スキル攻撃の攻撃範囲を取得
    /// </summary>
    /// <param name="skillAttackType"> スキル攻撃のタイプ</param>
    /// <returns> 攻撃範囲</returns>
    protected List<Vector2Int> GetSkillAttackRange(SkillAttackType skillAttackType)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        switch(skillAttackType)
        {
            case SkillAttackType.HeavyAttack:
                ret = GetHeavyAttackRange();
                break;

            case SkillAttackType.RotaryAttack:
                ret = GetRotaryAttackRange();
                break;

            case SkillAttackType.ThrustAttack:
                ret = GetThrustAttackRange();
                break;

            case SkillAttackType.MowDownAttack:
                ret = GetMowDownAttackRange();
                break;

            default:
                Debug.LogError("攻撃範囲エラー" + skillAttackType);
                break;
        }

        return ret;
    }

    /// <summary>
    /// 強攻撃
    /// </summary>
    /// <returns> 獲得経験値</returns>
    protected int HeavyAttack()
    {
        _animator.SetTrigger("HeavyAttackTrigger");

        return SkillAttack(SkillAttackType.HeavyAttack);
    }

    /// <summary>
    /// 強攻撃の攻撃範囲を取得
    /// </summary>
    /// <returns> 攻撃範囲</returns>
    private List<Vector2Int> GetHeavyAttackRange()
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        ret.Add(GetFrontPosition());
        return ret;
    }

    /// <summary>
    /// 回転攻撃
    /// </summary>
    /// <returns> 獲得経験値</returns>
    protected int RotaryAttack()
    {
        _animator.SetTrigger("RotaryAttackTrigger");

        return SkillAttack(SkillAttackType.RotaryAttack);
    }

    /// <summary>
    /// 回転攻撃の攻撃範囲を取得
    /// </summary>
    /// <returns> 攻撃範囲</returns>
    private List<Vector2Int> GetRotaryAttackRange()
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        var dir = _dir;

        for (int i = 0; i < _dir.Max(); i++)
        {
            ret.Add(pos + dir.ToVector2Int());
            dir = dir.Addition();
        }

        return ret;
    }

    /// <summary>
    /// 突き攻撃
    /// </summary>
    /// <returns> 獲得経験値</returns>
    protected int ThrustAttack()
    {
        _animator.SetTrigger("ThrustAttackTrigger");

        return SkillAttack(SkillAttackType.ThrustAttack);
    }

    /// <summary>
    /// 突き攻撃の攻撃範囲を取得
    /// </summary>
    /// <returns> 攻撃範囲</returns>
    private List<Vector2Int> GetThrustAttackRange()
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        Vector2Int frontPos = GetFrontPosition();

        ret.Add(frontPos);
        ret.Add(frontPos + _dir.ToVector2Int());

        return ret;
    }

    /// <summary>
    /// 薙ぎ払い攻撃
    /// </summary>
    /// <returns> 獲得経験値</returns>
    protected int MowDownAttack()
    {
        _animator.SetTrigger("MowDownAttackTrigger");

        return SkillAttack(SkillAttackType.MowDownAttack);
    }

    /// <summary>
    /// 薙ぎ払い攻撃の攻撃範囲を取得
    /// </summary>
    /// <returns> 攻撃範囲</returns>
    private List<Vector2Int> GetMowDownAttackRange()
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        ret.Add(pos + _dir.Subtraction().ToVector2Int());
        ret.Add(GetFrontPosition());
        ret.Add(pos + _dir.Addition().ToVector2Int());

        return ret;
    }

    /// <summary>
    /// ダメージ計算
    /// </summary>
    /// <param name="atk"> 攻撃力</param>
    /// <param name="def"> 防御力</param>
    /// <returns> ダメージ量</returns>
    private int DamageCalculation(int atk, int def)
    {
        int ret = atk - def;
        if(ret <= 0)
        {
            ret = 1;
        }
        return ret;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"> ダメージ量</param>
    /// <param name="def"> ダメージに対する抵抗デフォルトは0</param>
    /// <returns> trueなら死亡</returns>
    public bool Damage(int damage,int def = 0)
    {
        damage = DamageCalculation(damage, def);

        _hp -= damage;

        string str = string.Format(_name + "は、<color=#ff0000>{0:d}ダメージ</color>を受けた", damage);
        UIManager.instance.AddText(str);

        _animator.SetTrigger("DamageTrigger");

        if (_hp <= 0)
        {
            Death();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    protected virtual void Death()
    {
        UIManager.instance.AddText(_name + "は、やられた");
    }

    /// <summary>
    /// 体力の最大値を超えないように体力を増やす
    /// </summary>
    /// <param name="healValue"></param> 回復量
    /// <returns></returns> 実際の回復量
    private int HpAdd(int healValue)
    {
        _hp += healValue;
        if (_hp > _maxHp)
        {
            int over = _hp - _maxHp;
            _hp = _maxHp;
            return healValue - over;
        }

        return healValue;
    }

    /// <summary>
    /// 回復する
    /// </summary>
    /// <param name="healValue"></param> 回復量
    public void Heal(int healValue)
    {
        healValue = HpAdd(healValue);

        UIManager.instance.AddText(_name + "は、" + healValue + "回復した");
    }

    /// <summary>
    /// 自動回復
    /// </summary>
    public void Regeneration()
    {
        HpAdd(_regeneration);
    }

    /// <summary>
    /// CP増加
    /// </summary>
    /// <param name="value"> 増加値</param>
    /// <returns></returns>
    public virtual int CpAdd(int value)
    {
        _cp += value;

        return value;
    }

    /// <summary>
    /// CP減少
    /// </summary>
    /// <param name="value"> 減少値</param>
    /// <returns></returns>
    public int CpSub(int value)
    {
        _cp -= value;
        if (_cp < 0)
        {
            int over = -_cp;
            _cp = 0;
            return value - over;
        }

        return value;
    }

    /// <summary>
    /// 足元のイベントを実行
    /// </summary>
    protected void FootExecution()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        var terrain = _dungeonManager._floor.GetTerrainData(pos.x, pos.y);
        if (terrain == Floor.TerrainType.Event)
        {
            _dungeonManager._floor.EventExecution(pos.x, pos.y, this);
        }
        else if(terrain == Floor.TerrainType.Item)
        {
            PickUpItem(pos);
        }
    }

    /// <summary>
    /// アイテムを拾う
    /// </summary>
    protected virtual void PickUpItem(Vector2Int pos)
    {
        if (_itam == null)
        {
            _itam = _dungeonManager._floor.GetItemData(pos);
            _itam.BePickedUp();

            UIManager.instance.AddText(_name + "は、" + _itam._name + "を拾った");
        }
    }

    /// <summary>
    /// ダンジョン内にスポーンする
    /// </summary>
    protected void Spawn()
    {
        _dungeonManager = DungeonManager.instance;

        Vector2Int pos;
        bool flag = true;
        int sectionNo;
        do
        {
            var sections = _dungeonManager._floor._sections;
            sectionNo = Random.Range(0, sections.Count);
            var room = sections[sectionNo]._roomData;

            pos = new Vector2Int(Random.Range(room.left, room.right + 1), -Random.Range(room.top, room.bottom + 1));

            if (_dungeonManager._floor.GetTerrainData(pos) == Floor.TerrainType.Floor)
            {
                if (_dungeonManager._floor.GetCharacterData(pos) == -1)
                {
                    flag = false;
                }
            }

        } while (flag);

        Init(pos, sectionNo);
    }

    protected virtual void Init(Vector2Int pos, int roomNo)
    {
        transform.position = new Vector3(pos.x, 0, pos.y);

        _roomNo = roomNo;

        _destination = transform.position;

        _thinkEnd = false;
        _actFuncOnce = false;

        DungeonManager.instance._floor.SetCharacterData(transform.position.x, transform.position.z, _id);
    }
}
