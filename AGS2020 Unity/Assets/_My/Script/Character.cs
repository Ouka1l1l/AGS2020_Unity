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
    Max
}

public abstract class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Non,
        Enemy,
        Player,
        Max
    }

    public CharacterType _type { get; protected set; }

    /// <summary>
    /// 移動先
    /// </summary>
    protected Vector3 _destination;

    bool MoveFlag = false;

    /// <summary>
    /// 行動終了フラグ
    /// </summary>
    public bool _turnEnd { get; protected set; }

    Dir _dir;

    /// <summary>
    /// 体力
    /// </summary>
    protected int _hp;

    protected int _level;

    // Start is called before the first frame update
    protected void Start()
    {
        _destination = transform.position;
        _dir = Dir.Bottom;
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);
        _turnEnd = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        if(MoveFlag)
        {
            Move();
            if(_destination == transform.position)
            {
                MoveFlag = false;
                DungeonManager.instance._level._characterData[(int)-transform.position.z, (int)transform.position.x] = _type;
                EventRaise();
            }
        }
    }

    protected Vector2Int GetFrontPosition()
    {
        Vector2Int ret = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        switch (_dir)
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
                ret.y--;
                ret.x--;
                break;

            default:
                Debug.LogError("CharacterFrontError");
                break;
        }

        return ret;
    }

    /// <summary>
    /// 移動先の設定
    /// </summary>
    /// <param name="dir"></param> 移動方向
    protected void SetDestination(Dir dir)
    {
        _dir = dir;
        Vector2Int tmpDestination = GetFrontPosition();

        transform.rotation = Quaternion.Euler(0, (float)dir, 0);
        if (DungeonManager.instance._level.GetTerrainData(tmpDestination.x, tmpDestination.y) != Level.TerrainType.Wall)
        {
            _destination = new Vector3(tmpDestination.x, _destination.y, tmpDestination.y);
            MoveFlag = true;
            //DungeonManager.instance._level._characterData[tmpDestination.y, tmpDestination.x] = CharacterType.Non;
        }
    }

    /// <summary>
    /// 移動する
    /// </summary>
    protected void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * 2.0f);
        if (_destination == transform.position)
        {
            MoveFlag = false;
            DungeonManager.instance._level._characterData[(int)-transform.position.z, (int)transform.position.x] = _type;
            EventRaise();

            TurnEnd();
        }
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    protected void Attack()
    {
        TextManager.instance.AddText(name + "の攻撃");
        //Vector2Int flontPos = GetFrontPosition();
        //if(DungeonManager.instance._level._characterData[flontPos.y, flontPos.x] == CharacterType.Enemy)
        //{

        //}
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"></param> ダメージ量
    public void Damage(int damage)
    {
        _hp -= damage;

        string str = string.Format(name + "は、{0:d}ダメージを受けた", damage);
        TextManager.instance.AddText(str);
    }

    private void EventRaise()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        if (DungeonManager.instance._level.GetTerrainData(pos.x, pos.y) == Level.TerrainType.Event)
        {
            DungeonManager.instance._level.EventRaise(pos.x, pos.y, this);
        }
    }

    protected void Spawn()
    {
        Vector2Int pos;
        bool flag = true;
        do
        {
            var sections = DungeonManager.instance._level._sections;
            int sectionNo = Random.Range(0, sections.Count);
            var room = sections[sectionNo]._roomData;

            pos = new Vector2Int(Random.Range(room.left, room.right + 1), -Random.Range(room.top, room.bottom + 1));

            if (DungeonManager.instance._level.GetTerrainData(pos) == Level.TerrainType.Floor)
            {
                flag = false;
            }

        } while (flag);

        transform.position = new Vector3(pos.x, 0, pos.y);
        _destination = transform.position;
    }

    /// <summary>
    /// ターン開始
    /// </summary>
    public void TurnStart()
    {
        _turnEnd = false;
    }

    /// <summary>
    /// 自身の行動終了
    /// </summary>
    private void TurnEnd()
    {
        _turnEnd = true;
    }
}
