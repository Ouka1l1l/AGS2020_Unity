using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "ScriptableObject/EnemyData"
    )]
public class EnemyData : ScriptableObject
{
    public List<EnemyStatus> enemyData = new List<EnemyStatus>();
}

[System.Serializable]
public struct EnemyStatus
{
    public string name;
    public int maxHp;
    public int atk;
    public int def;
    public int exp;
}
