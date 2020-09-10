using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class LevelUpBonu : MonoBehaviour
{
    /// <summary>
    /// ステータス表示キャンバス
    /// </summary>
    [SerializeField]
    private Canvas _statusCanvas;

    /// <summary>
    /// ボーナス表示キャンバス
    /// </summary>
    [SerializeField]
    private Canvas _bonusCanvas;

    /// <summary>
    /// ボーナスパネル
    /// </summary>
    [SerializeField]
    private Image[] _bonuPanels = new Image[3];

    [SerializeField]
    private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>(3);

    private List<LevelUpBonus> _levelUpBonus;

    [SerializeField]
    private Image _choicePanel;

    private int _choice;

    /// <summary>
    /// 1フレーム前の横入力
    /// </summary>
    private float _oldHorizontal;

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player _player;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private SkillMenu _skillMenu;

    /// <summary>
    /// 覚えるスキル表示用スキルパネル
    /// </summary>
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

    /// <summary>
    /// アニメーター
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// ステータス表示用テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI[] _statusText = new TextMeshProUGUI[4];

    /// <summary>
    /// レベルアップ前の最大HP
    /// </summary>
    private int _beforeHpMax;

    /// <summary>
    /// レベルアップ前のCP容量
    /// </summary>
    private int _beforeCpLimit;

    /// <summary>
    /// レベルアップ前の攻撃力
    /// </summary>
    private int _beforeAtk;

    /// <summary>
    /// レベルアップ前の防御力
    /// </summary>
    private int _beforeDef;

    /// <summary>
    /// 
    /// </summary>
    private Action _updater;

    private bool _levelUpEnd = false; 

    private void Awake()
    {
        _levelUpBonusTable = Resources.Load<LevelUpBonusTable>("ScriptableObject/LevelUpBonusTable").levelUpBonusTable;

        _skillData = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;

        _animator = GetComponent<Animator>();

        _player = DungeonManager.instance._player;

        _beforeHpMax = _player._maxHp;
        _beforeCpLimit = _player._cpLimit;
        _beforeAtk = _player._atk;
        _beforeDef = _player._def;
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        _levelText.text = String.Format("Level {0:d}", _player._level);

        _statusText[0].text = string.Format("最大HP {0:d} +{1:d}", _beforeHpMax, _player._maxHp - _beforeHpMax);
        _statusText[1].text = string.Format("CP容量 {0:d} +{1:d}", _beforeCpLimit, _player._cpLimit - _beforeCpLimit);
        _statusText[2].text = string.Format("攻撃力 {0:d} +{1:d}", _beforeAtk, _player._atk - _beforeAtk);
        _statusText[3].text = string.Format("防御力 {0:d} +{1:d}", _beforeDef, _player._def - _beforeDef);

        _skillPanel.gameObject.SetActive(false);

        _oldHorizontal = Input.GetAxis("Horizontal");
        _choice = 0;

        BonusLottery();

        _statusCanvas.gameObject.SetActive(true);
        _bonusCanvas.gameObject.SetActive(false);

        _levelUpEnd = false;
        _updater = CheckStatus;
    }

    /// <summary>
    /// ボーナス内容抽選
    /// </summary>
    private void BonusLottery()
    {
        _levelUpBonus = new List<LevelUpBonus>();

        var levelUpBonusData = ChoiceLevelUpBonus();
        for (int i = 0; i < _texts.Count; i++)
        {
            //再抽選フラグ
            bool reLotteryFlag;

            //抽選番号
            int lotteryNo;

            do
            {
                reLotteryFlag = false;
                lotteryNo = Random.Range(0, levelUpBonusData.Count);
                var data = levelUpBonusData[lotteryNo];
                //ボーナス内容チェック
                foreach (var bonus in _levelUpBonus)
                {
                    if (bonus._bonusType == data._bonusType)
                    {
                        //ボーナス内容が被った
                        reLotteryFlag = true;
                        break;
                    }
                }

                //ボーナス内容がスキルだった場合
                if (data._bonusType == LevelUpBonusType.Skill && !reLotteryFlag)
                {
                    //すでに覚えてるスキルと被っていないかチェック
                    foreach (var slot in _player._skillAttackSlot)
                    {
                        if (data._val == (int)slot)
                        {
                            reLotteryFlag = true;
                            break;
                        }
                    }
                }
            }
            while (reLotteryFlag);

            _levelUpBonus.Add(levelUpBonusData[lotteryNo]);
            _texts[i].text = BonusText(levelUpBonusData[lotteryNo]);
        }
    }

    /// <summary>
    /// 使用するボーナス内容テーブルを選択
    /// </summary>
    /// <returns> 使用するボーナス内容テーブル</returns>
    private List<LevelUpBonus> ChoiceLevelUpBonus()
    {
        int index = _player._level / 5;
        if (index >= _levelUpBonusTable.Count)
        {
            index = _levelUpBonusTable.Count - 1;
        }
        return _levelUpBonusTable[index].levelUpBonus;
    }

    private void OnDisable()
    {
        _beforeHpMax = _player._maxHp;
        _beforeCpLimit = _player._cpLimit;
        _beforeAtk = _player._atk;
        _beforeDef = _player._def;
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

    public IEnumerator LevelUpBonus()
    {
        while(!_levelUpEnd)
        {
            _updater();
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void Submit()
    {
        SoundManager.instance.PlaySE("決定");

        if (_levelUpBonus[_choice]._bonusType != LevelUpBonusType.Skill)
        {
            _player.LevelUpBonus(_levelUpBonus[_choice]);

            LevelUpEnd();
        }
        else
        {
            ///////////
            _skillMenu.gameObject.SetActive(true);
            for (int i = 0; i < _player._skillAttackSlot.Length; i++)
            {
                _skillMenu.SetSkill(i, (int)_player._skillAttackSlot[i]);
            }
            //////////

            _skillPanel.gameObject.SetActive(true);
            _skillPanel.SetSkill(_skillData[_levelUpBonus[_choice]._val]);

            _updater = SkillSet;
        }
    }

    /// <summary>
    /// スキルをスロットにセット
    /// </summary>
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

            _skillMenu.gameObject.SetActive(false);

            _skillPanel.gameObject.SetActive(false);

            LevelUpEnd();
        }
    }

    /// <summary>
    /// ステータスを確認
    /// </summary>
    private void CheckStatus()
    {
        //アニメーションが終わるまで待機
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("LevelUpWait")
          || _animator.IsInTransition(0))
        {
            return;
        }

        if (Input.GetButtonDown("Submit"))
        {
            SoundManager.instance.PlaySE("決定");

            _statusText[0].text = string.Format("最大HP {0:d}", _player._maxHp);
            _statusText[1].text = string.Format("CP容量 {0:d}", _player._cpLimit);
            _statusText[2].text = string.Format("攻撃力 {0:d}", _player._atk);
            _statusText[3].text = string.Format("防御力 {0:d}", _player._def);

            _updater = CheckAfterStatus;
        }
    }

    private void CheckAfterStatus()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SoundManager.instance.PlaySE("決定");

            _statusCanvas.gameObject.SetActive(false);
            _bonusCanvas.gameObject.SetActive(true);

            _updater = SelectBonus;
        }
    }

    /// <summary>
    /// ボーナスを選択
    /// </summary>
    private void SelectBonus()
    {
        if (Input.GetButtonDown("Submit"))
        {
            Submit();
        }

        float horizontal = Input.GetAxis("Horizontal");

        if (_oldHorizontal == 0)
        {
            if (horizontal > 0)
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

    /// <summary>
    /// レベルアップ終了
    /// </summary>
    private void LevelUpEnd()
    {
        _animator.SetTrigger("OutTrigger");

        _updater = OutWait;
    }

    private void OutWait()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LevelUpEnd")
          && !_animator.IsInTransition(0))
        {
            _levelUpEnd = true;
        }
    }
}