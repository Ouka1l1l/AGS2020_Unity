using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowingItem : Item
{
    private Quaternion _defRotation;

    private const int _throwingDistance = 5;

    /// <summary>
    /// ダメージ量
    /// </summary>
    protected int _damageVal;

    protected override void Start()
    {
        _defRotation = transform.rotation;

        base.Start();
    }

    public override void Use(Character character)
    {
        UIManager.instance.AddText(character._name + "は、" + _name + "を投げた");

        SoundManager.instance.PlaySE("投擲");

        foreach (var renderer in _renderers)
        {
            renderer.enabled = true;
        }

        transform.rotation = Quaternion.Euler(0, (float)character._dir, 0);

        var throwData = ThrowSimulation(character);

        StartCoroutine(Throwing(throwData.Item2, throwData.Item1));
    }

    /// <summary>
    /// 投擲したときに当るキャラクタの番号と目標地点を返す
    /// </summary>
    /// <param name="character"> 投擲するキャラクタ</param>
    /// <returns> 当るキャラクタの番号と目標地点　:　キャラクタの番号が-1だった場合当たってない</returns>
    private System.Tuple<int,Vector3> ThrowSimulation(Character character)
    {
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

        return new System.Tuple<int, Vector3>(characterNo, throwingPos);
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

        if (characterNo != -1)
        {
            //当たったキャラ
            Character target;
            if (characterNo == 0)
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

            transform.rotation = _defRotation;
        }

        DungeonManager.instance.PauseEnd();
    }

    public override bool EnemyWhetherToUse(Enemy enemy)
    {
        if(ThrowSimulation(enemy).Item1 != -1)
        {
            return true;
        }

        return false;
    }
}
