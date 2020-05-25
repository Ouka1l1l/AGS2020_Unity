using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour
{
    [SerializeField]
    private Image[] _skillPanels = new Image[4];

    [SerializeField]
    private TextMeshProUGUI[] _skillNames = new TextMeshProUGUI[4];

    [SerializeField]
    private Image[] _skillIcons = new Image[4];

    [SerializeField]
    private TextMeshProUGUI[] _skillCosts = new TextMeshProUGUI[4];

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
        if(Input.GetButtonUp("R_Shoulder"))
        {
            gameObject.SetActive(false);
        }
    }

    public void SetSkill(int slotNo,int skillId)
    {
        if (skillId > 0 && 4 > slotNo && slotNo >= 0)
        {
            _skillPanels[slotNo].gameObject.SetActive(true);
            _skillNames[slotNo].text = _data[skillId].name;
            _skillIcons[slotNo].sprite = _data[skillId].icon;
            _skillCosts[slotNo].text = _data[skillId].cost.ToString();
        }
        else
        {
            Debug.LogError("スキルセットエラー" + skillId);
        }
    }
}
