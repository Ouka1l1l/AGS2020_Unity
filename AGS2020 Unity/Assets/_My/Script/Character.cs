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

public class Character : MonoBehaviour
{
    /// <summary>
    /// 移動先
    /// </summary>
    protected Vector3 _destination;

    /// <summary>
    /// 体力
    /// </summary>
    protected int _hp;

    // Start is called before the first frame update
    protected void Start()
    {
        _destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 移動先の設定
    /// </summary>
    /// <param name="dir"></param> 移動方向
    protected void SetDestination(Dir dir)
    {
        Vector3 tmpDestination = _destination;

        switch (dir)
        {
            case Dir.Top:
                tmpDestination.z++;
                break;

            case Dir.TopRight:
                tmpDestination.z++;
                tmpDestination.x++;
                break;

            case Dir.Right:
                tmpDestination.x++;
                break;

            case Dir.BottomRight:
                tmpDestination.z--;
                tmpDestination.x++;
                break;

            case Dir.Bottom:
                tmpDestination.z--;
                break;

            case Dir.BottomLeft:
                tmpDestination.z--;
                tmpDestination.x--;
                break;

            case Dir.Left:
                tmpDestination.x--;
                break;

            case Dir.TopLeft:
                tmpDestination.z--;
                tmpDestination.x--;
                break;

            default:
                Debug.LogError("CharacterError");
                break;
        }

        if (DungeonManager.instance._level.GetTerrainData((int)tmpDestination.x, (int)-tmpDestination.z) == Level.TerrainData.floor)
        {
            _destination = tmpDestination;
            transform.rotation = Quaternion.Euler(0, (float)dir, 0);
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
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"></param> ダメージ量
    void Damage(int damage)
    {
        _hp -= damage;
    }
}
