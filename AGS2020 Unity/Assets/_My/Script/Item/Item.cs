﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// アイテムのタイプ　enum
    /// </summary>
    public enum ItemType
    {
        MedicalBox
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
        _renderer = GetComponentInChildren<Renderer>();
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
        var level = DungeonManager.instance._level;

        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        level.SetTerrainData(pos.x, pos.y, Level.TerrainType.Floor);
        level.SetItemData(pos.x, pos.y, null);

        _renderer.enabled = false;
    }

    /// <summary>
    /// アイテムを捨てる
    /// </summary>
    /// <param name="pos"></param> 座標
    public void Drop(Vector2Int pos)
    {
        var level = DungeonManager.instance._level;

        var surroundingTerrainData = level.GetSurroundingTerrainData(pos.x, pos.y, 1, 1);

        Func<Vector2Int, bool> DropCheck = (Vector2Int offset) =>
        {
            if (surroundingTerrainData[offset] == Level.TerrainType.Floor)
            {
                SetPos(pos.x, pos.y);
                _renderer.enabled = true;
                StartCoroutine(DropMove(pos + offset));
                level.SetTerrainData(pos + offset, Level.TerrainType.Item);
                level.SetItemData(pos + offset, this);

                return true;
            }
            return false;
        };

        if(DropCheck(Vector2Int.zero))
        {
            return;
        }
        else
        {
            Dir dir = Dir.Top;
            for (int i = 0; i < 2; i++)
            {
                for (; dir <= Dir.TopLeft; dir += 90)
                {
                    if (DropCheck(dir.ToVector2Int()))
                    {
                        return;
                    }
                }
                dir = Dir.TopRight;
            }
        }

        Destroy(gameObject);
    }
    public void Drop(float x, float y)
    {
        Drop(new Vector2Int((int)x, (int)y));
    }

    private IEnumerator DropMove(Vector2Int dropPos)
    {
        DungeonManager.instance.PauseStart();

        float defY = transform.position.y;

        Vector3 target = new Vector3(dropPos.x, transform.position.y, dropPos.y);

        float dropDistance = Vector3.Distance(transform.position, target);

        target.y += 0.5f;

        while (transform.position != target)
        {
            float distance = Vector3.Distance(transform.position, target);

            if(distance <= dropDistance / 2)
            {
                target.y = defY;
            }
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);
            yield return null;
        }

        transform.SetY(defY);
        SetPos(dropPos.x, dropPos.y);

        DungeonManager.instance.PauseEnd();
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
