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
    private int _itamMax = 11;

    public int _cpLimit { get; private set; }

    private Rect _visibleRect;

    public SkillAttackType[] _skillAttackSlot { get; private set; }

    public UnityEvent _maskEvent { get; private set; } = new UnityEvent();

    /// <summary>
    /// 八方向の矢印
    /// </summary>
    [SerializeField]
    private GameObject _arrow;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _atk = 30;

        _type = CharacterType.Player;
        _id = 0;
        _name = "<color=#ffff00>プレイヤー</color>";

        _itemList = new List<Item>();

        _regeneration = 1;

        _skillAttackSlot = new SkillAttackType[4];

        UIManager.instance.GetSkillMenu().SetSkill(0, (int)SkillAttackType.ThrustAttack);
        UIManager.instance.GetSkillMenu().SetSkill(1, (int)SkillAttackType.RotaryAttack);
        UIManager.instance.GetSkillMenu().SetSkill(2, (int)SkillAttackType.HeavyAttack);
        UIManager.instance.GetSkillMenu().SetSkill(3, (int)SkillAttackType.MowDownAttack);

        _skillAttackSlot[0] = SkillAttackType.ThrustAttack;
        _skillAttackSlot[1] = SkillAttackType.RotaryAttack;
        _skillAttackSlot[2] = SkillAttackType.HeavyAttack;
        _skillAttackSlot[3] = SkillAttackType.MowDownAttack;

        _visibleRect = new Rect(2, 2, 2, 2);

        _cpLimit = 100;
    }

    public override bool Think()
    {
        if(Input.GetKeyDown("4"))
        {
            StartCoroutine(ExpUp(300));
        }

        if (_thinkEnd)
        {
            return true;
        }

        if (Input.GetButton("R_Shoulder"))
        {
            //移動せずに方向だけ変更
            _arrow.SetActive(true);
            Dir dir;
            if (GetInputDir(out dir))
            {
                _dir = dir;
                transform.rotation = Quaternion.Euler(0, (float)dir, 0);
                var arrowRot = _arrow.transform.rotation;
                _arrow.transform.rotation = Quaternion.Euler(arrowRot.x, 0, arrowRot.z);
            }
            return false;
        }

        if(Input.GetButtonUp("R_Shoulder"))
        {
            _arrow.SetActive(false);
        }

        if (Input.GetButtonDown("L_Shoulder"))
        {
            //スキル攻撃メニューを表示
            UIManager.instance.GetSkillMenu().gameObject.SetActive(true);
        }

        if(Input.GetButton("L_Shoulder"))
        {
            //スキル攻撃
            SkillAttackChoice();
            return false;
        }

        if (Input.GetButtonUp("L_Shoulder"))
        {
            //スキル攻撃メニューを非表示
            UIManager.instance.GetSkillMenu().gameObject.SetActive(false);
        }

        if (Input.GetButtonDown("Y_Button"))
        {
            //メニューを開く
            UIManager.instance.OpenMenu();
            return false;
        }

        if (Input.GetButtonDown("B_Button"))
        {
            //攻撃
            SetActFunc(AttackAction);
            return false;
        }
        else
        {
            //移動方向の入力
            Dir dir;
            if (GetInputDir(out dir))
            {
                //移動先の設定
                SetDestination(dir);
            }
        }

        return false;
    }

    protected override void AttackAction()
    {
        int exp = Attack();
        if (exp > 0)
        {
            StartCoroutine(ExpUp(exp));
        }
    }

    protected override void ActEnd()
    {
        base.ActEnd();

        CpAdd(1);

        if(_cp < _cpLimit)
        {
            Regeneration();
        }
    }

    public override bool Move()
    {
        if(base.Move())
        {
            var level = _dungeonManager._floor;
            if (_roomNo == -1)
            {
                level.UpdateMiniMap((int)transform.position.x, (int)transform.position.z);

                Vector2Int playerPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

                _visibleRect = new Rect(playerPos.y, playerPos.x, playerPos.y, playerPos.x);

                for (int i = 1; i <= 2; i++)
                {
                    if(level.GetTerrainData(playerPos.x + i,playerPos.y) == Floor.TerrainType.Road)
                    {
                        _visibleRect.right = playerPos.x + i;
                    }

                    if (level.GetTerrainData(playerPos.x - i, playerPos.y) == Floor.TerrainType.Road)
                    {
                        _visibleRect.left = playerPos.x - i;
                    }

                    if (level.GetTerrainData(playerPos.x, playerPos.y + i) == Floor.TerrainType.Road)
                    {
                        _visibleRect.bottom = playerPos.y + i;
                    }

                    if (level.GetTerrainData(playerPos.x, playerPos.y - i) == Floor.TerrainType.Road)
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

    /// <summary>
    /// スキル攻撃を選択
    /// </summary>
    private void SkillAttackChoice()
    {
        Func<int, bool> SlotChoice = (int slotNo) =>
          {
              var skillAttackType = _skillAttackSlot[slotNo];

              if (skillAttackType == SkillAttackType.Non)
              {
                  return false;
              }

              SetSkillAttackAction(skillAttackType);

              return true;
          };

        bool flag = false;
        if(Input.GetButtonDown("Y_Button"))
        {
            flag = SlotChoice(0);
        }
        else if(Input.GetButtonDown("B_Button"))
        {
            flag = SlotChoice(1);
        }
        else if(Input.GetButtonDown("A_Button"))
        {
            flag = SlotChoice(2);
        }
        else if(Input.GetButtonDown("X_Button"))
        {
            flag = SlotChoice(3);
        }

        if(flag)
        {
            UIManager.instance.GetSkillMenu().gameObject.SetActive(false);
        }
    }

    protected override void SkillAttackAction()
    {
        int exp = SkillAttackFunc();
        if (exp > 0)
        {
            StartCoroutine(ExpUp(exp));
        }

        SkillAttackFunc = null;
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
    /// <param name="index"> 使用するアイテムの番号</param>
    public void UseItem(int index)
    {
        _itemList[index].Use(this);
        _itemList.RemoveAt(index);

        _thinkEnd = true;
    }

    public void FootEvent()
    {
        FootExecution();
        _thinkEnd = true;
    }

    /// <summary>
    /// 経験値を増加
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    private IEnumerator ExpUp(int exp)
    {
        DungeonManager.instance.PauseStart();

        _exp += exp;
        while (_exp >= _nextLevelExp)
        {
            _exp -= _nextLevelExp;
            LevelUp();

            yield return StartCoroutine(UIManager.instance.OpenLevelUpBonusPanel());
        }

        DungeonManager.instance.PauseEnd();
    }

    private void LevelUp()
    {
        _level++;
        UIManager.instance.AddText(_name + "は、レベル" + _level + "になった");

        _nextLevelExp += 10;
        _maxHp += 10;
        _cpLimit += 10;
        _atk++;
        _def++;
    }

    public void LevelUpBonus(LevelUpBonus bonus)
    {
        switch(bonus._bonusType)
        {
            case LevelUpBonusType.HpUp:
                _maxHp += bonus._val;
                break;
            case LevelUpBonusType.CpUp:
                _cpLimit += bonus._val;
                break;
            case LevelUpBonusType.AtkUp:
                _atk += bonus._val;
                break;
            case LevelUpBonusType.DefUp:
                _def += bonus._val;
                break;
            case LevelUpBonusType.Skill:
                break;
            default:
                break;
        }
    }

    public void SetSkillSlot(int slotNo,int skillNo)
    {
        UIManager.instance.GetSkillMenu().SetSkill(slotNo, skillNo);
        _skillAttackSlot[slotNo] = (SkillAttackType)skillNo;

    }

    new public void Spawn()
    {
        base.Spawn();

        /////////デバック
        _dungeonManager._floor.SetCharacterData(transform.position.x, transform.position.z, -1);

        Vector2Int pos = _dungeonManager._floor.staisPos;
        pos.x++;

        transform.position = new Vector3(pos.x, 0, pos.y);
        _destination = transform.position;

        _dungeonManager._floor.SetCharacterData(transform.position.x, transform.position.z, _id);
        _roomNo = _dungeonManager._floor.GetRoomNo(transform.position.x, transform.position.z);
        ////////

        _dungeonManager._floor.UpdateMiniMap(_roomNo);

        Camera.main.GetComponent<FollowCamera>().SetTarget(gameObject);
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

        _dungeonManager.GameOver();
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
        var level = _dungeonManager._floor;

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

                if (level.GetTerrainData(pos.x + x, pos.z) == Floor.TerrainType.Road)
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

                    if (level.GetTerrainData(pos.x, pos.z + y) == Floor.TerrainType.Road)
                    {
                        ret = true;
                    }
                    else
                    {
                        if (level.GetTerrainData(pos.x + x, pos.z + y) == Floor.TerrainType.Road)
                        {
                            ret = true;
                        }
                    }
                }
            }
        }
        else
        {
            var room = _dungeonManager._floor._sections[_roomNo]._roomData;

            if (room.left - 1 <= pos.x && pos.x <= room.right + 1
                && room.top - 1 <= -pos.z && -pos.z <= room.bottom + 1)
            {
                ret = true;
            }
        }

        return ret;
    }
}
