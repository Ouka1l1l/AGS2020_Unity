using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelUpBonusTable",
    menuName = "ScriptableObject/LevelUpBonusTable"
    )]
public class LevelUpBonusTable : ScriptableObject
{
    public List<LevelUpBonusData> levelUpBonusTable = new List<LevelUpBonusData>();
}

public enum LevelUpBonusType
{
    HpUp,
    CpUp,
    AtkUp,
    DefUp,
    Skill
}

[System.Serializable]
public struct LevelUpBonus
{
    public LevelUpBonusType _bonusType;
    public int _val;
}

[System.Serializable]
public struct LevelUpBonusData
{
    public List<LevelUpBonus> levelUpBonus;
}