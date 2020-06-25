using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusMenu : Menu
{
    [SerializeField]
    private TextMeshProUGUI _AtkText;

    [SerializeField]
    private TextMeshProUGUI _DefText;

    [SerializeField]
    private List<SkillPanel> _skillPanels = new List<SkillPanel>(4);

    private List<SkillAttack> _data;

    private void Awake()
    {
        _data = Resources.Load<SkillAttackData>("ScriptableObject/SkillAttackData").skillAttackData;
    }

    // Start is called before the first frame update
    new void Start()
    {
        _AtkText.text = "";
        _DefText.text = "";
    }

    new protected void OnEnable()
    {
        DungeonManager.instance.PauseStart();

        var player = DungeonManager.instance._player;

        for (int i = 0; i < _skillPanels.Count; i++)
        {
            _skillPanels[i].SetSkill(_data[(int)player._skillAttackSlot[i]]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
        {
            UIManager.instance.CloseMenu();
        }

        var player = DungeonManager.instance._player;

        _AtkText.text = player._atk.ToString();

        _DefText.text = player._def.ToString();
    }
}
