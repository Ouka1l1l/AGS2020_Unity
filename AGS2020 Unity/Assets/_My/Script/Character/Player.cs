﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    /// <summary>
    /// アイテムリスト
    /// </summary>
    public List<Item> _itemList { get; private set; }

    /// <summary>
    /// アイテムの所持上限
    /// </summary>
    private int _itamMax = 1;

    public enum SkillAttack
    {
        Non,
        RotaryAttack
    }

    public SkillAttack[] _skillAttacks { get; private set; }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _atk = 30;

        _type = CharacterType.Player;
        _id = 0;
        _name = "プレイヤー";

        _itemList = new List<Item>();

        _regeneration = 1;

        _skillAttacks = new SkillAttack[4];

        _skillAttacks[0] = SkillAttack.RotaryAttack;
        _skillAttacks[3] = SkillAttack.RotaryAttack;
    }

    public override bool Think()
    {
        if(_action != Action.Non)
        {
            return true;
        }

        if(Input.GetButtonDown("Menu"))
        {
            UIManager.instance.OpenMenu();
            return false;
        }

        if(Input.GetButtonDown("Special"))
        {
            RotaryAttack();
        }

        if (Input.GetButtonDown("Attack"))
        {
            int exp = Attack();
            if (exp > 0)
            {
                ExpUp(exp);
            }
            return false;
        }
        else
        {
            Dir dir;
            if (GetInputDir(out dir))
            {
                SetDestination(dir);
            }
        }

        return false;
    }

    new public bool Act()
    {
        return base.Act();
    }

    protected override void PickUpItem(Vector2Int pos)
    {
        if (_itemList.Count < _itamMax)
        {
            base.PickUpItem(pos);

            //拾ったアイテムをアイテムリストにしまう
            _itemList.Add(_itam);
            _itam = null;
        }
    }

    /// <summary>
    /// アイテムを使用
    /// </summary>
    /// <param name="index"></param> 使用するアイテムの番号
    public void UseItem(int index)
    {
        _itemList[index].Use(this);
        _itemList.RemoveAt(index);
        _action = Action.Item;
    }

    public void FootEvent()
    {
        _action = Action.Move;
    }

    private void ExpUp(int exp)
    {
        _exp += exp;
        while(_exp >= _nextLevelExp)
        {
            LevelUp();
            _exp -= _nextLevelExp;
        }
    }

    private void LevelUp()
    {
        _level++;
        UIManager.instance.AddText(_name + "は、" + _level + "になった");

        _maxHp += 10;
        _nextLevelExp += 10;
        _atk++;
        _def++;
    }

    new public void Spawn()
    {
        base.Spawn();

        DungeonManager.instance._level.SetCharacterData(transform.position.x, transform.position.z, -1);

        Vector2Int pos = DungeonManager.instance._level.staisPos;
        pos.x++;

        transform.position = new Vector3(pos.x, 0, pos.y);
        _destination = transform.position;

        DungeonManager.instance._level.SetCharacterData(transform.position.x, transform.position.z, _id);

        Camera.main.GetComponent<FollowCamera>().SetTarget(this);
    }

    /// <summary>
    /// 入力値に遊びを持たせる
    /// </summary>
    /// <param name="input"></param> 入力値
    /// <returns></returns> 遊びの値を超えたら1.0f 未満なら0.0f
    private float InputDeadZone(float input)
    {
        const float deadZone = 0.5f;

        float ret = 0.0f;
        if (Mathf.Abs(input) >= deadZone)
        {
            if(input > 0)
            {
                ret = 1.0f;
            }
            else
            {
                ret = -1.0f;
            }
        }
        return ret;
    }

    /// <summary>
    /// 入力情報から向きを取得
    /// </summary>
    /// <param name="dir"></param> 向き
    /// <returns></returns> 入力があったか
    private bool GetInputDir(out Dir dir)
    {
        float h = Input.GetAxis("Horizontal");
        h = InputDeadZone(h);

        float v = Input.GetAxis("Vertical");
        v = InputDeadZone(v);

        if(h == 0.0f && v == 0.0f)
        {
            dir = Dir.Bottom;
            return false;
        }

        var angle = Mathf.Atan2(h, v);
        angle *= Mathf.Rad2Deg;

        if(angle < 0)
        {
            angle += 360;
        }

        dir = (Dir)angle;
        return true;
    }

    protected override void Death()
    {
        base.Death();

        StartCoroutine(DungeonManager.instance.ReStart());
    }
}
