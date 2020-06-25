using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpBonu : MonoBehaviour
{
    [SerializeField]
    private Image[] _bonuPanels = new Image[3];

    [SerializeField]
    private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>(3);

    private List<LevelUpBonus> _levelUpBonus;

    [SerializeField]
    private Image _choicePanel;

    private int _choice;

    private float _oldHorizontal;

    private Player _player;

    private int _level;

    private bool _skillSet;

    [SerializeField]
    private SkillPanel _skillPanel;

    /// <summary>
    /// レベルアップボーナスのテーブル
    /// </summary>
    private List<LevelUpBonusData> _levelUpBonusTable;

    /// <summary>
    /// スキル攻撃のデータ
    /// </summary>
    private List<SkillAttack> _skillData;

    private void Awake()
    {
        _levelUpBonusTable = Resources.Load<LevelUpBonusTable>("ScriptableObject/LevelUpBonusTable").levelUpBonusTable;

        _skillData = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;

        _level = 1;
    }

    private void Start()
    {
        _player = DungeonManager.instance._player;
    }

    private void OnEnable()
    {
        DungeonManager.instance.PauseStart();

        _level++;

        _skillSet = false;
        _skillPanel.gameObject.SetActive(false);

        _oldHorizontal = Input.GetAxis("Horizontal");
        _choice = 0;

        _levelUpBonus = new List<LevelUpBonus>();

        var levelUpBonusData = ChoiceLevelUpBonus();
        for (int i = 0; i < _texts.Count; i++)
        {
            int r;
            bool flag;
            do
            {
                flag = false;
                r = Random.Range(0, levelUpBonusData.Count);
                foreach (var l in _levelUpBonus)
                {
                    var data = levelUpBonusData[r];
                    if (l._bonusType == data._bonusType)
                    {
                        flag = true;
                    }
                    else
                    {
                        if(data._bonusType == LevelUpBonusType.Skill)
                        {
                            foreach(var slot in _player._skillAttackSlot)
                            {
                                if(data._val == (int)slot)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            while (flag);

            _levelUpBonus.Add(levelUpBonusData[r]);
            _texts[i].text = BonusText(levelUpBonusData[r]);
        }
    }

    private List<LevelUpBonus> ChoiceLevelUpBonus()
    {
        int index = _level / 5;
        if (index >= _levelUpBonusTable.Count)
        {
            index = _levelUpBonusTable.Count - 1;
        }
        return _levelUpBonusTable[index].levelUpBonus;
    }

    private void OnDisable()
    {
        DungeonManager.instance.PauseEnd();

        if(_level != _player._level)
        {
            gameObject.SetActive(true);
        }
    }

    private string BonusText(LevelUpBonus levelUpBonus)
    {
        string text = "";
        switch(levelUpBonus._bonusType)
        {
            case LevelUpBonusType.HpUp:
                text = "HP最大値\n";
                break;
            case LevelUpBonusType.CpUp:
                text = "CP最大値\n";
                break;
            case LevelUpBonusType.AtkUp:
                text = "攻撃力\n";
                break;
            case LevelUpBonusType.DefUp:
                text = "防御力\n";
                break;
            case LevelUpBonusType.Skill:
                text = _skillData[levelUpBonus._val].name;
                break;
            default:
                break;
        }

        if(levelUpBonus._bonusType != LevelUpBonusType.Skill)
        {
            text += "+" + levelUpBonus._val;
        }

        return text;
    }

    // Update is called once per frame
    void Update()
    {
        if(_skillSet)
        {
            SkillSet();
            return;
        }

        if(Input.GetButtonDown("Submit"))
        {
            Submit();
        }

        float horizontal = Input.GetAxis("Horizontal");

        if(_oldHorizontal == 0)
        {
            if(horizontal > 0)
            {
                if (_choice < (_bonuPanels.Length - 1))
                {
                    _choice++;
                }
            }
            else if (horizontal < 0)
            {
                if (_choice > 0)
                {
                    _choice--;
                }
            }
        }

        _choicePanel.transform.position = _bonuPanels[_choice].transform.position;

        _oldHorizontal = horizontal;
    }

    private void Submit()
    {
        if (_levelUpBonus[_choice]._bonusType != LevelUpBonusType.Skill)
        {
            _player.LevelUpBonus(_levelUpBonus[_choice]);
            gameObject.SetActive(false);
        }
        else
        {
            UIManager.instance.GetSkillMenu().gameObject.SetActive(true);
            _skillPanel.gameObject.SetActive(true);
            _skillPanel.SetSkill(_skillData[_levelUpBonus[_choice]._val]);
            _skillSet = true;
        }
    }

    private void SkillSet()
    {
        int slotNo = -1;
        if (Input.GetButtonDown("Y_Button"))
        {
            slotNo = 0;
        }
        else if (Input.GetButtonDown("B_Button"))
        {
            slotNo = 1;
        }
        else if (Input.GetButtonDown("A_Button"))
        {
            slotNo = 2;
        }
        else if (Input.GetButtonDown("X_Button"))
        {
            slotNo = 3;
        }

        if(slotNo != -1)
        {
            _player.SetSkillSlot(slotNo, _levelUpBonus[_choice]._val);
            UIManager.instance.GetSkillMenu().gameObject.SetActive(false);
            _skillPanel.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}