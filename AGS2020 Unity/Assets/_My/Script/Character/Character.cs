﻿using System.Collections;
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
    public static int Max(this Dir dir)
    {
        return System.Enum.GetValues(typeof(Dir)).Length;
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
    protected Dir _dir;

    /// <summary>
    /// 体力
    /// </summary>
    protected int _hp = 100;

    /// <summary>
    /// レベル
    /// </summary>
    protected int _level = 1;

    /// <summary>
    /// 経験値
    /// </summary>
    protected int _exp;

    /// <summary>
    /// 攻撃力
    /// </summary>
    protected int _atk = 10;

    /// <summary>
    /// 防御力
    /// </summary>
    protected int _def = 5;

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
    /// 自身から見てターゲットはどの方向にいるか
    /// </summary>
    /// <param name="targetPos"></param> ターゲットの座標
    /// <returns></returns> 自身から見たターゲットの方向
    protected Dir GetTargetDir(Vector3 targetPos)
    {
        Vector2Int vec = new Vector2Int((int)(targetPos.x - transform.position.x), (int)(targetPos.z - transform.position.z));

        string str = "";
        if(vec.y == 1)
        {
            str = "Top";
        }
        else if(vec.y == -1)
        {
            str = "Bottom";
        }

        if(vec.x == -1)
        {
            str += "Left";
        }
        else if(vec.x == 1)
        {
            str += "Right";
        }

        return (Dir)System.Enum.Parse(typeof(Dir), str);
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
                DungeonManager.instance._level.SetCharacterData(tmpDestination.x, tmpDestination.y, _id);
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
            EventRaise();

            TurnEnd();
        }
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    protected void Attack()
    {
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);

        TextManager.instance.AddText(name + "の攻撃");

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
            target.Damage(damage);
        }

        TurnEnd();
    }

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