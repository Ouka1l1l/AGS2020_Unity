using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FloorData",
    menuName = "ScriptableObject/FloorData"
    )]
public class FloorDatas : ScriptableObject
{
    [SerializeField]
    private List<FloorData> _floorDatas = new List<FloorData>();

    public GameObject GetLotteryEnemy(int index)
    {
        var enemyList = _floorDatas[index]._enemyList;
        var r =  Random.Range(0, enemyList.Count);

        return enemyList[r].gameObject;
    }

    public GameObject GetLotteryItem(int index)
    {
        var enemyList = _floorDatas[index]._itemList;
        var r = Random.Range(0, enemyList.Count);

        return enemyList[r].gameObject;
    }

    public GameObject GetLotteryTrap(int index)
    {
        var enemyList = _floorDatas[index]._trapList;
        var r = Random.Range(0, enemyList.Count);

        return enemyList[r].gameObject;
    }
}

[System.Serializable]
public struct FloorData
{
    public List<Enemy> _enemyList;
    public List<Item> _itemList;
    public List<Event> _trapList;
}