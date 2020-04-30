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
    public int _hierarchy { get; private set; } = 0;

    public Player _player { get; private set; }

    /// <summary>
    /// エネミーのターン開始トリガー
    /// </summary>
    private bool _enemyTrigger;

    /// <summary>
    /// 現在のターン数
    /// </summary>
    public int _turnCount { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _turnCount = 1;
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

            if (_enemyTrigger)
            {
                //エネミーのターン開始
                foreach (var enemy in _level._enemies)
                {
                    enemy.TurnStart();
                }
                _enemyTrigger = false;
            }

            //全員のターンが終了しているかチェック
            bool enemyTurnEnd = true;
            foreach (var enemy in _level._enemies)
            {
                if(!enemy._turnEnd)
                {
                    enemyTurnEnd = false;
                    break;
                }
            }

            if(enemyTurnEnd)
            {
                //プレイヤーのターン開始
                _player.TurnStart();
                _enemyTrigger = true;
                _turnCount++;
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
        _enemyTrigger = true;
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
