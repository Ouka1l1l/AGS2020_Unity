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

    Dir _dir;

    /// <summary>
    /// 体力
    /// </summary>
    protected int _hp;

    // Start is called before the first frame update
    protected void Start()
    {
        _destination = transform.position;
        _dir = Dir.Bottom;
        transform.rotation = Quaternion.Euler(0, (float)_dir, 0);
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
        if (DungeonManager.instance._level.GetTerrainData(tmpDestination.x, -tmpDestination.y) != Level.TerrainType.Wall)
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
    void Damage(int damage)
    {
        _hp -= damage;
    }

    private void EventRaise()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, -(int)transform.position.z);

        if (DungeonManager.instance._level._terrainData[pos.y, pos.x] == Level.TerrainType.Event)
        {
            DungeonManager.instance._level._eventData[pos.y, pos.x].Raise(this);
        }
    }
}
