using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour
{
    [SerializeField]
    private Image[] skillPanels = new Image[4];

    [SerializeField]
    private TextMeshProUGUI[] skillNames = new TextMeshProUGUI[4];

    [SerializeField]
    private TextMeshProUGUI[] skillCosts = new TextMeshProUGUI[4];

    private List<SkillAttack> _data;

    // Start is called before the first frame update
    void Start()
    {
        _data = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;
    }

    // Update is called once per frame
    void Update()
    {
        var playerSkill = DungeonManager.instance._player._skillAttacks;

        for (int s = 0; s < 4; s++)
        {
            int skillId = (int)playerSkill[s];
            if (skillId > 0)
            {
                skillId--;
                skillPanels[s].gameObject.SetActive(true);
                skillNames[s].text = _data[skillId].name;
                skillCosts[s].text = _data[skillId].cost.ToString();
            }
            else
            {
                skillPanels[s].gameObject.SetActive(false);
            }
        }
    }
}
