using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public int _cpLimit { get; private set; }

    private Rect _visibleRect;

    public SkillAttackType[] _skillAttackSlot { get; private set; }

    public UnityEvent _maskEvent { get; private set; } = new UnityEvent();

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

        _skillAttackSlot = new SkillAttackType[4];

        UIManager.instance.GetSkillMenu().SetSkill(1, (int)SkillAttackType.RotaryAttack);

        _skillAttackSlot[1] = SkillAttackType.RotaryAttack;

        _visibleRect = new Rect(2, 2, 2, 2);

        _cpLimit = 100;
    }

    public override bool Think()
    {
        if(_action != Action.Non)
        {
            return true;
        }

        if (Input.GetButtonDown("R_Shoulder"))
        {
            UIManager.instance.GetSkillMenu().gameObject.SetActive(true);
        }

        if(Input.GetButton("R_Shoulder"))
        {
            SkillAttackChoice();
            return false;
        }

        if (Input.GetButtonDown("Y_Button"))
        {
            UIManager.instance.OpenMenu();
            return false;
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

    public void ActEnd(int turnCount)
    {
        if((turnCount % 1) == 0)
        {
            CpAdd(1);
        }

        if(_cp <= _cpLimit)
        {
            Regeneration();
        }
    }

    protected override bool Move()
    {
        if(base.Move())
        {
            var level = _dungeonManager._level;
            if (_roomNo == -1)
            {
                level.UpdateMiniMap((int)transform.position.x, (int)transform.position.z);

                Vector2Int playerPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

                _visibleRect = new Rect(playerPos.y, playerPos.x, playerPos.y, playerPos.x);

                for (int i = 1; i <= 2; i++)
                {
                    if(level.GetTerrainData(playerPos.x + i,playerPos.y) == Level.TerrainType.Road)
                    {
                        _visibleRect.right = playerPos.x + i;
                    }

                    if (level.GetTerrainData(playerPos.x - i, playerPos.y) == Level.TerrainType.Road)
                    {
                        _visibleRect.left = playerPos.x - i;
                    }

                    if (level.GetTerrainData(playerPos.x, playerPos.y + i) == Level.TerrainType.Road)
                    {
                        _visibleRect.bottom = playerPos.y + i;
                    }

                    if (level.GetTerrainData(playerPos.x, playerPos.y - i) == Level.TerrainType.Road)
                    {
                        _visibleRect.top = playerPos.y - i;
                    }
                }
            }
            else
            {
                level.UpdateMiniMap(_roomNo);
            }

            _maskEvent.Invoke();

            return true;
        }

        return false;
    }

    private void SkillAttackChoice()
    {
        Func<int> SkillAttack = null;

        Func<int, bool> SlotChoice = (int slotNo) =>
          {
              var skillAttackType = _skillAttackSlot[slotNo];
              if (_skillAttackData[(int)skillAttackType].cost > 0)
              {
                  switch(skillAttackType)
                  {
                      case Player.SkillAttackType.RotaryAttack:
                          SkillAttack = RotaryAttack;
                          break;

                      default:
                          Debug.LogError("技選択エラー" + skillAttackType);
                          break;
                  }
              }

              return false;
          };

        if(Input.GetButtonDown("Y_Button"))
        {
            SlotChoice(0);
        }
        else if(Input.GetButtonDown("B_Button"))
        {
            SlotChoice(1);
        }
        else if(Input.GetButtonDown("A_Button"))
        {
            SlotChoice(2);
        }
        else if(Input.GetButtonDown("X_Button"))
        {
            SlotChoice(3);
        }

        if (SkillAttack != null)
        {
            int exp = SkillAttack();
            if (exp > 0)
            {
                ExpUp(exp);
            }
        }
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
            _exp -= _nextLevelExp;
            LevelUp();
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

        _dungeonManager._level.SetCharacterData(transform.position.x, transform.position.z, -1);

        /////////デバック
        Vector2Int pos = _dungeonManager._level.staisPos;
        pos.x++;

        transform.position = new Vector3(pos.x, 0, pos.y);
        _destination = transform.position;

        _dungeonManager._level.SetCharacterData(transform.position.x, transform.position.z, _id);
        _roomNo = _dungeonManager._level.GetRoomNo(transform.position.x, transform.position.z);
        ////////

        _dungeonManager._level.UpdateMiniMap(_roomNo);

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

        StartCoroutine(_dungeonManager.ReStart());
    }

    public override int CpAdd(int value)
    {
        base.CpAdd(value);
        if (_cp > _cpLimit)
        {
            int over = _cp - _cpLimit;
            _cp = _cpLimit;

            Damage(over);

            return value - over;
        }

        return value;
    }

    /// <summary>
    /// 見えているかのチェック
    /// </summary>
    /// <param name="pos"></param> 座標
    /// <returns></returns>
    public bool VisibilityCheck(Vector3 pos)
    {
        var level = _dungeonManager._level;

        bool ret = false;

        if (_roomNo == -1)
        {
            Vector2Int playerPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

            if (_visibleRect.left - 1 <= pos.x && pos.x <= _visibleRect.right + 1
                && _visibleRect.top - 1 <= pos.z && pos.z <= _visibleRect.bottom + 1)
            {
                int x = 0;
                if (playerPos.x < pos.x)
                {
                    x--;
                }
                else if (playerPos.x > pos.x)
                {
                    x++;
                }

                if (level.GetTerrainData(pos.x + x, pos.z) == Level.TerrainType.Road)
                {
                    ret = true;
                }
                else
                {

                    int y = 0;
                    if (playerPos.y < pos.z)
                    {
                        y--;
                    }
                    else if (playerPos.y > pos.z)
                    {
                        y++;
                    }

                    if (level.GetTerrainData(pos.x, pos.z + y) == Level.TerrainType.Road)
                    {
                        ret = true;
                    }
                    else
                    {
                        if (level.GetTerrainData(pos.x + x, pos.z + y) == Level.TerrainType.Road)
                        {
                            ret = true;
                        }
                    }
                }
            }
        }
        else
        {
            var room = _dungeonManager._level._sections[_roomNo]._roomData;

            if (room.left - 1 <= pos.x && pos.x <= room.right + 1
                && room.top - 1 <= -pos.z && -pos.z <= room.bottom + 1)
            {
                ret = true;
            }
        }

        return ret;
    }
}
