using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    /// <summary>
    /// アイテムリスト
    /// </summary>
    private List<Item> _itemList;

    /// <summary>
    /// アイテムの所持上限
    /// </summary>
    private int _itamMax = 2;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _atk = 20;

        _type = CharacterType.Player;
        _id = 0;
        _name = "プレイヤー";

        _itemList = new List<Item>();

        _regeneration = 1;
    }

    public override bool Think()
    {
        if (_destination == transform.position)
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                if(_itam != null)
                {
                    return UseItem();
                }
                else
                {
                    Debug.Log("持ってない");
                }
            }

            if (Input.GetAxis("Attack") > 0)
            {
                Attack();
                return true;
            }
            else
            {
                Dir dir;
                if (GetInputDir(out dir))
                {
                    return SetDestination(dir);
                }
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
