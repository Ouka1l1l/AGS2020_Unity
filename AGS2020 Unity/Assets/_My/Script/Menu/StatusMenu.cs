using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusMenu : BaseMenu
{
    [SerializeField]
    private Button _statusButton;

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
    private void Start()
    {
        _AtkText.text = "";
        _DefText.text = "";
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Color color = _statusButton.colors.normalColor;
        color.a = _statusButton.colors.selectedColor.a;
        var colorBlock = _statusButton.colors;
        colorBlock.normalColor = color;
        _statusButton.colors = colorBlock;

        var player = DungeonManager.instance._player;

        for (int i = 0; i < _skillPanels.Count; i++)
        {
            _skillPanels[i].SetSkill(_data[(int)player._skillAttackSlot[i]]);
        }

        _AtkText.text = player._atk.ToString();

        _DefText.text = player._def.ToString();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Color color = _statusButton.colors.normalColor;
        color.a = 0;
        var colorBlock = _statusButton.colors;
        colorBlock.normalColor = color;
        _statusButton.colors = colorBlock;

        _statusButton.Select();
        _statusButton.OnSelect(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
        {
            UIManager.instance.CloseMenu();
        }
    }
}
