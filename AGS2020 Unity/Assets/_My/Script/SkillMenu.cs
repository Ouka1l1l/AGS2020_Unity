using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour
{
    [SerializeField]
    private SkillPanel[] _skillPanels = new SkillPanel[4];

    private List<SkillAttack> _data;

    private void Awake()
    {
        _data = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;

        foreach (var skillPanel in _skillPanels)
        {
            skillPanel.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSkill(int slotNo,int skillId)
    {
        if (skillId > 0 && 4 > slotNo && slotNo >= 0)
        {
            _skillPanels[slotNo].gameObject.SetActive(true);
            _skillPanels[slotNo].SetSkill(_data[skillId]);
        }
        else
        {
            Debug.LogError("スキルセットエラー" + skillId);
        }
    }
}
