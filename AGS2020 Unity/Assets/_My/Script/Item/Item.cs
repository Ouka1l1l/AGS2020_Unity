using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// アイテムのタイプ　enum
    /// </summary>
    public enum ItemType
    {
        Portion
    }

    /// <summary>
    /// アイテムのタイプ
    /// </summary>
    public ItemType _type { get; protected set; }

    /// <summary>
    /// アイテム名
    /// </summary>
    public string _name { get; protected set; }

    /// <summary>
    /// レンダラー
    /// </summary>
    private Renderer _renderer;

    // Start is called before the first frame update
    protected void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// 座標を変更する
    /// </summary>
    /// <param name="x"></param> X座標
    /// <param name="y"></param> Y座標
    public void SetPos(int x, int y)
    {
        transform.position = new Vector3(x, transform.position.y, y);
    }

    /// <summary>
    /// 拾われる
    /// </summary>
    public void BePickedUp()
    {
        _renderer.enabled = false;
    }

    /// <summary>
    /// アイテムを使用する
    /// </summary>
    /// <param name="character"></param> 使ったキャラ
    public virtual void Use(Character character)
    {
        UIManager.instance.AddText(character._name + "は、" + _name + "を使った");
    }
}
