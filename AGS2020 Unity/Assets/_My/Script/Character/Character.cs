using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector2Int ret = new Vector2Int(); ;
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
    protected int _id;

    /// <summary>
    /// キャラの名前
    /// </summary>
    public string _name { get; protected set; }

    /// <summary>
    /// 移動先
    /// </summary>
    protected Vector3 _destination;

    /// <summary>
    /// 行動enum
    /// </summary>
    protected enum Action
    {
        Non,
        Wait,
        Move,
        Attack,
        SkillAttack,
        Item
    }

    /// <summary>
    /// 行う行動
    /// </summary>
    protected Action _action;

    /// <summary>
    /// 現在いるの部屋の区画番号 部屋にいない場合は-1
    /// </summary>
    public int _roomNo;

    /// <summary>
    /// 持っているアイテム
    /// </summary>
    public Item _itam;

    /// <summary>
    /// 自身の向き
    /// </summary>
    protected Dir _dir;

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

    public int _nextLevelExp { get; protected set; } = 100;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int _atk { get; protected set; } = 10;

    /// <summary>
    /// 防御力
    /// </summary>
    public int _def { get; protected set; } = 5;

    /// <summary>
    /// 自動回復量
    /// </summary>
    protected int _regeneration;

    /// <summary>
    /// 技のデータ
    /// </summary>
    protected List<SkillAttack> _skillAttackData;

    // Start is called before the first frame update
    protected void Start()
    {
        _hp = _maxHp;
        _destination = transform.position;
        _dir = Dir.Bottom;
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        _skillAttackData = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;
    }

    /// <summary>
    /// 行動を決定する
    /// </summary>
    /// <returns></returns> true 行動決定
    public abstract bool Think();

    /// <summary>
    /// 行動を実行する
    /// </summary>
    /// <returns></returns> true 行動終了
    public bool Act()
    {
        bool end = false;
        switch(_action)
        {
            case Action.Wait:
                end = true;
                break;

            case Action.Move:
                end = Move();
                break;

            case Action.Attack:
            case Action.SkillAttack:
                end = true;
                break;

            case Action.Item:
                end = true;
                break;

            default:
                Debug.LogError(_name + "Actエラー" + _action);
                end = true;
                break;
        }

        if(end)
        {
            _action = Action.Non;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 正面の座標を取得
    /// </summary>
    /// <returns></returns> 正面の座標
    protected Vector2Int GetFrontPosition()
    {
        Vector2Int ret = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        ret += _dir.ToVector2Int();

        return ret;
    }

    /// <summary>
    /// 自身から見てターゲットはどの方向にいるか
    /// </summary>
    /// <param name="targetPos"></param> ターゲットの座標
    /// <returns></returns> 自身から見たターゲットの方向
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
    /// <param name="dir"></param> 移動方向
    protected bool SetDestination(Dir dir)
    {
        _dir = dir;
        Vector2Int tmpDestination = GetFrontPosition();

        var level = DungeonManager.instance._level;

        transform.rotation = Quaternion.Euler(0, (float)dir, 0);
        if (level.GetTerrainData(tmpDestination) != Level.TerrainType.Wall)
        {
            if (level.GetCharacterData(tmpDestination) == -1)
            {
                _destination = new Vector3(tmpDestination.x, _destination.y, tmpDestination.y);
                level.SetCharacterData(transform.position.x, transform.position.z, -1);
                DungeonManager.instance._level.SetCharacterData(tmpDestination.x, tmpDestination.y, _id);

                _action = Action.Move;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 移動する
    /// </summary>
    protected virtual bool Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * 5.0f);
        if (_destination == transform.position)
        {
            _roomNo = DungeonManager.instance._level.GetRoomNo(transform.position.x, transform.position.z);
            FootExecution();

            return true;
        }

        return false;
    }

    /// <summary>
    /// アイテムを使う
    /// </summary>
    /// <returns></returns>
    protected void UseItem()
    {
        _itam.Use(this);
        _action = Action.Item;
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    /// <returns></returns> 経験値
    protected int Attack()
    {
        _action = Action.Attack;

        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        UIManager.instance.AddText(_name + "の攻撃");

        var dungeonManager = DungeonManager.instance;
        Vector2Int frontPos = GetFrontPosition();
        var characterNo = dungeonManager._level.GetCharacterData(frontPos);
        if(characterNo != -1)
        {
            Character target;
            if(characterNo == 0)
            {
                target = dungeonManager._player;
            }
            else
            {
                target = dungeonManager._level._enemies[characterNo - 1];
            }
            int damage = DamageCalculation(target._def);
            if(target.Damage(damage))
            {
                return target._exp;
            }
        }

        return 0;
    }

    protected int RotaryAttack()
    {
        _action = Action.SkillAttack;

        UIManager.instance.AddText(_name + "の回転切り");
        var dungeonManager = DungeonManager.instance;

        int ret = 0;

        var characterData = dungeonManager._level.GetSurroundingCharacterData(transform.position.x, transform.position.z, 1, 1);
        foreach(var charData in characterData)
        {
            if (charData.Value != -1)
            {
                Character target = null;
                if (_id == 0)
                {
                    if (charData.Value > 0)
                    {
                        target = dungeonManager._level._enemies[charData.Value - 1];
                    }
                }
                else
                {
                    if (charData.Value == 0)
                    {
                        target = dungeonManager._player;
                    }
                }

                if (target != null)
                {
                    int damage = DamageCalculation(target._def);
                    if (target.Damage(damage))
                    {
                        ret += target._exp;
                    }
                }
            }
        }

        return ret;
    }

    /// <summary>
    /// ダメージ計算
    /// </summary>
    /// <param name="def"></param>
    /// <returns></returns>
    private int DamageCalculation(int def)
    {
        int ret = _atk - def;
        if(ret <= 0)
        {
            ret = 1;
        }
        return ret;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"></param> ダメージ量
    /// <returns></returns> trueなら死亡
    public bool Damage(int damage)
    {
        _hp -= damage;

        string str = string.Format(_name + "は、{0:d}ダメージを受けた", damage);
        UIManager.instance.AddText(str);

        if(_hp <= 0)
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
    /// 足元のイベントを実行
    /// </summary>
    private void FootExecution()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        var terrain = DungeonManager.instance._level.GetTerrainData(pos.x, pos.y);
        if (terrain == Level.TerrainType.Event)
        {
            DungeonManager.instance._level.EventExecution(pos.x, pos.y, this);
        }
        else if(terrain == Level.TerrainType.Item)
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
            _itam = DungeonManager.instance._level.GetItemData(pos);
            _itam.BePickedUp();

            UIManager.instance.AddText(_name + "は、" + _itam._name + "を拾った");
        }
    }

    /// <summary>
    /// ダンジョン内にスポーンする
    /// </summary>
    protected void Spawn()
    {
        Vector2Int pos;
        bool flag = true;
        int sectionNo;
        do
        {
            var sections = DungeonManager.instance._level._sections;
            sectionNo = Random.Range(0, sections.Count);
            var room = sections[sectionNo]._roomData;

            pos = new Vector2Int(Random.Range(room.left, room.right + 1), -Random.Range(room.top, room.bottom + 1));

            if (DungeonManager.instance._level.GetTerrainData(pos) == Level.TerrainType.Floor)
            {
                if (DungeonManager.instance._level.GetCharacterData(pos) == -1)
                {
                    flag = false;
                }
            }

        } while (flag);

        transform.position = new Vector3(pos.x, 0, pos.y);
        _destination = transform.position;
        _roomNo = sectionNo;

        _action = Action.Non;

        DungeonManager.instance._level.SetCharacterData(transform.position.x, transform.position.z, _id);
    }
}
