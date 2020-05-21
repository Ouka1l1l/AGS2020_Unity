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
    /// レンダー
    /// </summary>
    protected Renderer _renderer;

    protected Renderer _iconRenderer;

    protected void Start()
    {
        _renderer = GetComponent<Renderer>();
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
    public void SetPos(int x,int y)
    {
        transform.position = new Vector3(x, transform.position.y, y);
    }

    /// <summary>
    /// イベント実行
    /// </summary>
    /// <param name="character"></param> イベントの影響を受けるキャラクタ
    public virtual void Execution(Character character)
    {
        _renderer.enabled = true;
        _iconRenderer.enabled = true;
        UIManager.instance.AddText(character._name + "は、" + _name + "を踏んだ");
    }
}
