using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SkillAttackData",
    menuName = "ScriptableObject/SkillAttackData"
    )]
public class SkillAttackData : ScriptableObject
{
    public List<SkillAttack> skillAttackData = new List<SkillAttack>();
}

[System.Serializable]
public struct SkillAttack
{
    public string name;
    public int cost;
}
