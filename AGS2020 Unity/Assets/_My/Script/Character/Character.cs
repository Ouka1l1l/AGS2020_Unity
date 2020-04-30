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
    Max = 8
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
    /// 移動先
    /// </summary>
    protected Vector3 _destination;

    bool MoveFlag = false;

    /// <summary>
    /// 行動終了フラグ
    /// </summary>
    public bool _turnEnd { get; protected set; } = true;

    /// <summary>
    /// 自身の向き
    /// </summary>
    Dir _dir;

    /// <summary>
    /// 体力
    /// </summary>
    protected int _hp;

    /// <summary>
    /// レベル
    /// </summary>
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
        }
    }

    /// <summary>
    /// 正面の座標を取得
    /// </summary>
    /// <returns></returns> 正面の座標
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
                ret.y++;
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

        var level = DungeonManager.instance._level;

        transform.rotation = Quaternion.Euler(0, (float)dir, 0);
        if (level.GetTerrainData(tmpDestination) != Level.TerrainType.Wall)
        {
            if (level.GetCharacterData(tmpDestination) == -1)
            {
                _destination = new Vector3(tmpDestination.x, _destination.y, tmpDestination.y);
                MoveFlag = true;
                level.SetCharacterData(transform.position.x, transform.position.z, -1);
            }
        }
    }

    /// <summary>
    /// 移動する
    /// </summary>
    protected void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * 5.0f);
        if (_destination == transform.position)
        {
            MoveFlag = false;
            DungeonManager.instance._level.SetCharacterData(transform.position.x, transform.position.z, _id);
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

        var dungeonManager = DungeonManager.instance;
        Vector2Int frontPos = GetFrontPosition();
        var characterNo = dungeonManager._level.GetCharacterData(frontPos);
        if(characterNo != -1)
        {
            if(characterNo == 0)
            {
                dungeonManager._player.Damage(10);
            }
            else
            {
                dungeonManager._level._enemies[characterNo - 1].Damage(10);
            }
        }
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

    /// <summary>
    /// 足元のイベントを実行
    /// </summary>
    private void EventRaise()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        if (DungeonManager.instance._level.GetTerrainData(pos.x, pos.y) == Level.TerrainType.Event)
        {
            DungeonManager.instance._level.EventRaise(pos.x, pos.y, this);
        }
    }

    /// <summary>
    /// ダンジョン内にスポーンする
    /// </summary>
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
                if (DungeonManager.instance._level.GetCharacterData(pos) == -1)
                {
                    flag = false;
                }
            }

        } while (flag);

        transform.position = new Vector3(pos.x, 0, pos.y);
        _destination = transform.position;

        DungeonManager.instance._level.SetCharacterData(transform.position.x, transform.position.z, _id);
    }

    /// <summary>
    /// 自身のターン開始
    /// </summary>
    public void TurnStart()
    {
        TextManager.instance.AddText(name + "のターン");
        _turnEnd = false;
    }

    /// <summary>
    /// 自身のターン終了
    /// </summary>
    protected void TurnEnd()
    {
        _turnEnd = true;
    }
}
