﻿using System.Collections;
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
    /// 現在のターン数
    /// </summary>
    public int _turnCount { get; private set; }

    /// <summary>
    /// ターン制御用enum
    /// </summary>
    enum TurnControl
    {
        playerThink,
        playetAct,
        enemyThink,
        enemyAct
    }

    /// <summary>
    /// ターン制御用変数
    /// </summary>
    TurnControl _turnControl;

    /// <summary>
    /// ポーズフラグ
    /// </summary>
    bool _pauseFlag;

    // Start is called before the first frame update
    void Start()
    {
        _pauseFlag = false;
        _turnCount = 1;
        _player = Instantiate((GameObject)Resources.Load("Player")).GetComponent<Player>();
        _level = Instantiate((GameObject)Resources.Load("Level")).GetComponent<Level>();
        NextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            _pauseFlag = !_pauseFlag;
        }

        if(_pauseFlag)
        {
            return;
        }

        //ターン制御
        switch(_turnControl)
        {
            //プレイヤー思考待ち
            case TurnControl.playerThink:
                if(_player.Think())
                {
                    _turnControl = TurnControl.playetAct;
                }
                break;

            //プレイヤー行動中
            case TurnControl.playetAct:
                if(_player.Act())
                {
                    _turnControl = TurnControl.enemyThink;
                }
                break;

            //敵思考中
            case TurnControl.enemyThink:
                bool thinkEnd = true;
                foreach (var enemy in _level._enemies)
                {
                    if(!enemy.Think())
                    {
                        thinkEnd = false;
                    }
                }

                if (thinkEnd)
                {
                    _turnControl = TurnControl.enemyAct;
                }
                break;

            //敵行動中
            case TurnControl.enemyAct:

                bool actEnd = true;
                foreach (var enemy in _level._enemies)
                {
                    if(!enemy.Act())
                    {
                        actEnd = false;
                    }
                }

                if(actEnd)
                {
                    _turnControl = TurnControl.playerThink;
                    _turnCount++;
                }
                break;

            default:
                Debug.LogError("ターンエラー" + _turnControl);
                break;
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
        _turnControl = TurnControl.playerThink;
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
