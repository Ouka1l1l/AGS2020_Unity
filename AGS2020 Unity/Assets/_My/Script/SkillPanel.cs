using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _skillName;

    [SerializeField]
    private Image _skillIcon;

    [SerializeField]
    private TextMeshProUGUI _skillCost;

    [SerializeField]
    private TextMeshProUGUI _addAtk;

    /// <summary>
    /// スキルパネルをセット
    /// </summary>
    /// <param name="data"> セットするスキルのデータ</param>
    public void SetSkill(SkillAttack data)
    {
        _skillName.text = data.name;
        _skillIcon.sprite = data.icon;
        _skillCost.text = string.Format("CP{0,2:d}", data.cost); ;
        _addAtk.text = string.Format("攻+{0:d}", data.addAtk);
    }
}
