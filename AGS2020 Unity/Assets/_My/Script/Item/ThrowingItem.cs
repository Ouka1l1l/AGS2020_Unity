using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowingItem : Item
{
    private const int _throwingDistance = 5;

    /// <summary>
    /// ダメージ量
    /// </summary>
    protected int _damageVal;

    public override void Use(Character character)
    {
        UIManager.instance.AddText(character._name + "は、" + _name + "を投げた");

        foreach (var renderer in _renderers)
        {
            renderer.enabled = true;
        }

        transform.rotation = Quaternion.Euler(0, (float)character._dir, 0);

        var floor = DungeonManager.instance._floor;

        //正面ベクトル
        var frontVec = character._dir.ToVector2Int();

        SetPos((int)character.transform.position.x, (int)character.transform.position.z);

        //目標地点
        Vector3 throwingPos = transform.position;

        //当たったキャラの番号
        //-1なら当たっていない
        int characterNo = -1;

        //何かに当たるまで飛ばす
        for (int i = 0; i < _throwingDistance; i++)
        {
            var tmpPos = throwingPos;
            tmpPos.x += frontVec.x;
            tmpPos.z += frontVec.y;

            if (floor.GetTerrainData(tmpPos.x, tmpPos.z) == Floor.TerrainType.Wall)
            {
                //壁に当たった
                break;
            }
            else
            {
                characterNo = floor.GetCharacterData(tmpPos.x, tmpPos.z);
                if (characterNo != -1)
                {
                    //キャラに当たった

                    //敵の同士討ち防止
                    if (character._id != 0 && characterNo > 0)
                    {
                        characterNo = -1;
                    }
                    break;
                }
            }

            throwingPos = tmpPos;
        }

        StartCoroutine(Throwing(throwingPos, characterNo));
    }

    /// <summary>
    /// 投擲コルーチン
    /// </summary>
    /// <param name="throwingPos"> 投擲目標地点</param>
    /// <param name="characterNo"> 当たったキャラの番号 -1なら当たっていない</param>
    /// <returns></returns>
    private IEnumerator Throwing(Vector3 throwingPos, int characterNo)
    {
        DungeonManager.instance.PauseStart();

        //投擲目標地点まで移動
        while (transform.position != throwingPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, throwingPos, Time.deltaTime * 5);
            yield return null;
        }

        if(characterNo != -1)
        {
            //当たったキャラ
            Character target;
            if(characterNo == 0)
            {
                //プレイヤーに当たった
                target = DungeonManager.instance._player;
            }
            else
            {
                //敵に当たった
                target = DungeonManager.instance._floor._enemies[characterNo - 1];
            }

            //当たったキャラにダメージ
            target.Damage(_damageVal);

            Destroy(gameObject);
        }
        else
        {
            //地面に落とす
            Drop(transform.position.x, transform.position.z);
        }

        DungeonManager.instance.PauseEnd();
    }
}
