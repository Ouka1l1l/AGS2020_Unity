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
        Max
    }

    /// <summary>
    /// イベントタイプ
    /// </summary>
    public EventType _type { get; protected set; }

    public void SetPos(int x,int y)
    {
        transform.position = new Vector3(x, transform.position.y, -y);
    }

    /// <summary>
    /// イベント実行
    /// </summary>
    /// <param name="character"></param> イベントの影響を受けるキャラクタ
    public abstract void Raise(Character character);
}
