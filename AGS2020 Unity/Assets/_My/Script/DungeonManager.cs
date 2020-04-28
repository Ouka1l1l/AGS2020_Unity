using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    /// <summary>
    /// 階層
    /// </summary>
    public Level _level { get; private set; }

    /// <summary>
    /// 何階層目か
    /// </summary>
    private int _hierarchy = 0;

    public Player _player { get; private set; }

    /// <summary>
    /// 行動中のエネミー番号
    /// </summary>
    private int _enemyNo;

    /// <summary>
    /// 現在のターン数
    /// </summary>
    private int turnCount;

    // Start is called before the first frame update
    void Start()
    {
        turnCount = 1;
        _player = Instantiate((GameObject)Resources.Load("Player")).GetComponent<Player>();
        _level = Instantiate((GameObject)Resources.Load("Level")).GetComponent<Level>();
        NextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        //ターン制御
        if(!_player._turnEnd)
        {
            //プレイヤーのターン
        }
        else
        {
            //エネミーのターン

            //現在行動中のエネミー
            var enemy = _level._enemies[_enemyNo];
            if (!enemy._turnEnd)
            {

            }
            else
            {
                //現在行動中のエネミーのターン終了
                _enemyNo++;
                if(_enemyNo < _level._enemies.Count)
                {
                    //次のエネミーのターン開始
                    _level._enemies[_enemyNo].TurnStart();
                }
                else
                {
                    //プレイヤーのターン開始
                    _player.TurnStart();
                    _enemyNo = 0;
                    turnCount++;
                }
            }
        }
    }

    /// <summary>
    /// 次の階に進む
    /// </summary>
    public void NextLevel()
    {
        _hierarchy++;
        _level.CreateLevel(new Vector2Int(50, 50), 10);
        _player.Spawn();
    }

    /// <summary>
    /// 座標をマス目に変換
    /// </summary>
    /// <param name="x"></param> X座標
    /// <param name="y"></param> Y座標
    /// <returns></returns> マス目
    public Vector2Int GetGrid(int x, int y)
    {
        return new Vector2Int(x, -y);
    }
    public Vector2Int GetGrid(Vector2Int pos)
    {
        return GetGrid(pos.x, pos.y);
    }
    public Vector2Int GetGrid(float x, float y)
    {
        return GetGrid((int)x, (int)y);
    }
}
