using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event : MonoBehaviour
{
    /// <summary>
    /// イベントタイプ
    /// </summary>
    public enum EventType
    {
        Non,    //なし
        Stairs, //階段
        NeedleFloor, //トゲ床
        ExplosionFloor, //爆発床
        MonsterTrap,    //モンスター呼び寄せ
        Max
    }

    /// <summary>
    /// イベントタイプ
    /// </summary>
    public EventType _type { get; protected set; }

    /// <summary>
    /// イベント名
    /// </summary>
    protected string _name;

    /// <summary>
    /// 部屋番号
    /// </summary>
    protected int _roomNo;

    /// <summary>
    /// レンダー
    /// </summary>
    protected Renderer _renderer;

    protected Renderer _iconRenderer;

    protected virtual void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
        {
            //見えないように
            _renderer.enabled = false;
        }

        _iconRenderer = transform.Find("MiniMapIcon").GetComponent<Renderer>();
        if (_iconRenderer != null)
        {
            //見えないように
            _iconRenderer.enabled = false;
        }
    }

    /// <summary>
    /// 座標を変更する
    /// </summary>
    /// <param name="x"></param> X座標
    /// <param name="y"></param> Y座標
    /// <param name="roomNo"></param> 部屋番号
    public void Init(int x,int y,int roomNo)
    {
        transform.position = new Vector3(x, transform.position.y, y);
        _roomNo = roomNo;
    }

    /// <summary>
    /// イベント実行
    /// </summary>
    /// <param name="character"></param> イベントの影響を受けるキャラクタ
    public virtual void Execution(Character character)
    {
        if (DungeonManager.instance._player._roomNo == _roomNo)
        {
            _renderer.enabled = true;
            _iconRenderer.enabled = true;
        }
        UIManager.instance.AddText(character._name + "は、" + _name + "を踏んだ");
    }
}
