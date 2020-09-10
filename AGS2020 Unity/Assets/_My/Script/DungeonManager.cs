using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : Singleton<DungeonManager>
{
    [SerializeField]
    private GameOver _gameOver;

    [SerializeField]
    private Fade _fade;

    /// <summary>
    /// 階層
    /// </summary>
    public Floor _floor { get; private set; }

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
    /// ターン制御用
    /// </summary>
    Action TurncController;

    /// <summary>
    /// 行動中の敵
    /// </summary>
    int _actingEnemyNo = 0;

    /// <summary>
    /// ポーズフラグ
    /// </summary>
    private int _pause;

    // Start is called before the first frame update
    void Start()
    {
        _pause = 0;
        _turnCount = 1;
        _player = Instantiate((GameObject)Resources.Load("Player")).GetComponentInChildren<Player>();
        _floor = Instantiate((GameObject)Resources.Load("Level")).GetComponent<Floor>();

        SoundManager.instance.PlayBGM("ダンジョン");

        PauseStart();
        NextFloorFunc();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    _player.Damage(10);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    _player.CpSub(10);
        //}

        if (_pause > 0)
        {
            return;
        }

        //ターン制御
        TurncController();
    }

    /// <summary>
    /// プレイヤー行動決定
    /// </summary>
    private void PlayerThink()
    {
        if(_player.Think())
        {
            TurncController = PlayerMove;
        }
    }

    /// <summary>
    /// プレイヤーの移動
    /// </summary>
    private void PlayerMove()
    {
        if (_player.Move())
        {
            TurncController = PlayerAct;
        }
    }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    private void PlayerAct()
    {
        if (_player.Act())
        {
            TurncController = EnemyThink;
        }
    }

    /// <summary>
    /// 敵の行動決定
    /// </summary>
    private void EnemyThink()
    {
        //////////
        //TurncController = TurnEnd;
        //return;
        //////////

        bool thinkEnd = true;
        foreach (var enemy in _floor._enemies)
        {
            if (enemy != null)
            {
                if (!enemy.Think())
                {
                    thinkEnd = false;
                }
            }
        }

        if(thinkEnd)
        {
            TurncController = EnemyMove;
            _actingEnemyNo = 0;
        }
    }

    /// <summary>
    /// 敵の移動
    /// </summary>
    private void EnemyMove()
    {
        bool moveEnd = true;
        foreach (var enemy in _floor._enemies)
        {
            if (enemy != null)
            {
                if (!enemy.Move())
                {
                    moveEnd = false;
                }
            }
        }

        if(moveEnd)
        {
            TurncController = EnemyAct;
        }
    }

    /// <summary>
    /// 敵の行動
    /// </summary>
    private void EnemyAct()
    {
        var enemies = _floor._enemies;
        if (enemies[_actingEnemyNo] != null)
        {
            if(enemies[_actingEnemyNo].Act())
            {
                _actingEnemyNo++;
            }
        }
        else
        {
            _actingEnemyNo++;
        }

        if(_actingEnemyNo >= enemies.Count)
        {
            TurncController = TurnEnd;
        }
    }

    /// <summary>
    /// ターン終了時の処理
    /// </summary>
    private void TurnEnd()
    {
        if (_turnCount % 5 == 0)
        {
            _floor.EnemyIncrease();
        }

        _turnCount++;

        TurncController = PlayerThink;
    }

    /// <summary>
    /// 次の階に進む
    /// </summary>
    public void NextFloor()
    {
        PauseStart();

        _fade.FadeOut(() => NextFloorFunc());
    }

    private void NextFloorFunc()
    {
        if (_hierarchy >= 10)
        {
            SoundManager.instance.StopBGM();
            _fade.FadeOut(() => SceneManager.LoadScene("Clear"));
            return;
        }

        _hierarchy++;
        _floor.CreateFloor(new Vector2Int(50, 50), 10);
        _player.Spawn();
        TurncController = PlayerThink;

        UIManager.instance.WhatFloor(_hierarchy);
        UIManager.instance.TextClear();

        _fade.FadeIn(() => PauseEnd());
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

    /// <summary>
    /// ポーズ開始
    /// </summary>
    public void PauseStart()
    {
        _pause++;
    }

    /// <summary>
    /// ポーズ終了
    /// </summary>
    public void PauseEnd()
    {
        _pause--;
    }

    public void GameOver()
    {
        PauseStart();
        _gameOver.gameObject.SetActive(true);
    }
}
